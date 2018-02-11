using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using RESTar.Deflection;
using RESTar.Deflection.Dynamic;
using RESTar.Linq;
using RESTar.Requests;
using RESTar.Results.Success;
using Starcounter;

namespace RESTar.OData
{
    internal class MetadataDocument : OK
    {
        private const string ReadOnlyAnnotation = "<Annotation Term=\"Org.OData.Core.V1.Permissions\">" +
                                                  "<EnumMember>Org.OData.Core.V1.Permission/Read</EnumMember>" +
                                                  "</Annotation>";

        private const string WriteOnlyAnnotation = "<Annotation Term=\"Org.OData.Core.V1.Permissions\">" +
                                                   "<EnumMember>Org.OData.Core.V1.Permission/Write</EnumMember>" +
                                                   "</Annotation>";


        private static string InsertableAnnotation(bool state) => "<Annotation Term=\"Org.OData.Capabilities.V1.InsertRestrictions\">" +
                                                                  $"<Record><PropertyValue Bool=\"{state.XMLBool()}\" Property=\"Insertable\"/>" +
                                                                  "<PropertyValue Property=\"NonInsertableNavigationProperties\">" +
                                                                  "<Collection/></PropertyValue></Record></Annotation>";

        private static string UpdatableAnnotation(bool state) => "<Annotation Term=\"Org.OData.Capabilities.V1.UpdateRestrictions\">" +
                                                                 $"<Record><PropertyValue Bool=\"{state.XMLBool()}\" Property=\"Updatable\"/>" +
                                                                 "<PropertyValue Property=\"NonUpdatableNavigationProperties\">" +
                                                                 "<Collection/></PropertyValue></Record></Annotation>";

        private static string DeletableAnnotation(bool state) => "<Annotation Term=\"Org.OData.Capabilities.V1.DeleteRestrictions\">" +
                                                                 $"<Record><PropertyValue Bool=\"{state.XMLBool()}\" Property=\"Deletable\"/>" +
                                                                 "<PropertyValue Property=\"NonDeletableNavigationProperties\">" +
                                                                 "<Collection/></PropertyValue></Record></Annotation>";

        private const string EntityContainerName = "DefaultContainer";

        private static bool IsDynamicMember(Member member) => member.Type == typeof(object) || member.Type == typeof(JValue);

        internal MetadataDocument(Metadata metadata, ITraceable trace) : base(trace)
        {
            ContentType = "application/xml";
            Body = new MemoryStream();
            using (var swr = new StreamWriter(Body, Serialization.Serializer.UTF8, 1024, true))
            {
                swr.WriteXMLHeader();
                swr.Write("<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\"><edmx:DataServices>");
                swr.Write("<Schema Namespace=\"global\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">");

                var (enumTypes, complexTypes) = metadata.PeripheralTypes.Split(t => t.Type.IsEnum);

                foreach (var (enumType, _) in enumTypes)
                {
                    swr.Write($"<EnumType Name=\"{enumType.FullName}\">");
                    foreach (var member in EnumMember.GetMembers(enumType))
                        swr.Write($"<Member Name=\"{member.Name}\" Value=\"{member.Value}\"/>");
                    swr.Write("</EnumType>");
                }

                foreach (var (type, members) in complexTypes)
                {
                    var (dynamicMembers, declaredMembers) = members.Split(IsDynamicMember);
                    var isOpenType = type.IsDynamic() || dynamicMembers.Any();
                    swr.Write($"<ComplexType Name=\"{type.FullName}\" OpenType=\"{isOpenType.XMLBool()}\">");
                    foreach (var member in declaredMembers)
                    {
                        swr.Write($"<Property Name=\"{member.Name}\" Nullable=\"{member.Nullable.XMLBool()}\" " +
                                  $"Type=\"{member.Type.GetEdmTypeName()}\"");
                        if (member.ReadOnly)
                            swr.Write($">{ReadOnlyAnnotation}</Property>");
                        else if (member.WriteOnly)
                            swr.Write($">{WriteOnlyAnnotation}</Property>");
                        else swr.Write("/>");
                    }
                    swr.Write("</ComplexType>");
                }

                foreach (var (type, members) in metadata.EntityResourceTypes.Where(t => t.Type != typeof(Metadata)))
                {
                    var (dynamicMembers, declaredMembers) = members.Split(IsDynamicMember);
                    var isOpenType = type.IsDynamic() || dynamicMembers.Any();
                    swr.Write($"<EntityType Name=\"{type.FullName}\" OpenType=\"{isOpenType.XMLBool()}\">");
                    var key = declaredMembers.OfType<DeclaredProperty>().FirstOrDefault(p => p.IsKey);
                    if (key != null) swr.Write($"<Key><PropertyRef Name=\"{key.Name}\"/></Key>");
                    foreach (var member in declaredMembers.Where(p => !(p is DeclaredProperty d) || !d.Hidden || d.Equals(key)))
                    {
                        swr.Write($"<Property Name=\"{member.Name}\" Nullable=\"{member.Nullable.XMLBool()}\" " +
                                  $"Type=\"{member.Type.GetEdmTypeName()}\"");
                        if (member.ReadOnly)
                            swr.Write($">{ReadOnlyAnnotation}</Property>");
                        else if (member.WriteOnly)
                            swr.Write($">{WriteOnlyAnnotation}</Property>");
                        else swr.Write("/>");
                    }
                    swr.Write("</EntityType>");
                }

                swr.Write("<EntityType Name=\"RESTar.DynamicResource\" OpenType=\"true\"/>");
                swr.Write($"<EntityContainer Name=\"{EntityContainerName}\">");

                foreach (var entitySet in metadata.EntityResources.Where(t => t.Type != typeof(Metadata)))
                {
                    swr.Write($"<EntitySet EntityType=\"{entitySet.Type.GetEdmTypeName()}\" Name=\"{entitySet.Name}\">");
                    var methods = metadata.CurrentAccessScope[entitySet].Intersect(entitySet.AvailableMethods).ToList();
                    swr.Write(InsertableAnnotation(methods.Contains(Methods.POST)));
                    swr.Write(UpdatableAnnotation(methods.Contains(Methods.PATCH)));
                    swr.Write(DeletableAnnotation(methods.Contains(Methods.DELETE)));
                    swr.Write("</EntitySet>");
                }
                swr.Write("</EntityContainer>");
                swr.Write($"<Annotations Target=\"global.{EntityContainerName}\">");
                swr.Write("<Annotation Term=\"Org.OData.Capabilities.V1.ConformanceLevel\"><EnumMember>Org.OData.Capabilities.V1." +
                          "ConformanceLevelType/Minimal</EnumMember></Annotation>");
                swr.Write("<Annotation Term=\"Org.OData.Capabilities.V1.SupportedFormats\">");
                swr.Write("<Collection>");
                swr.Write("<String>application/json;odata.metadata=minimal;IEEE754Compatible=false;odata.streaming=true</String>");
                swr.Write("</Collection>");
                swr.Write("</Annotation>");
                swr.Write("<Annotation Bool=\"true\" Term=\"Org.OData.Capabilities.V1.AsynchronousRequestsSupported\"/>");
                swr.Write("<Annotation Term=\"Org.OData.Capabilities.V1.FilterFunctions\"><Collection></Collection></Annotation>");
                swr.Write("</Annotations>");
                swr.Write("</Schema>");
                swr.Write("</edmx:DataServices></edmx:Edmx>");
            }
            Body.Seek(0, SeekOrigin.Begin);
        }
    }

    internal static class MetadataExtensions
    {
        internal static void WriteXMLHeader(this StreamWriter swr)
        {
            swr.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }

        internal static string GetEdmTypeName(this Type type)
        {
            if (type.IsEnum) return "global." + type.FullName;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    switch (type)
                    {
                        case var _ when type == typeof(Binary): return "Edm.Binary";
                        case var _ when type == typeof(Guid): return "Edm.Guid";
                        case var _ when type.IsNullable(out var t): return GetEdmTypeName(t);
                        case var _ when type.Implements(typeof(IDictionary<,>), out var p) && p[0] == typeof(string):
                            return "global.RESTar.DynamicResource";
                        case var _ when typeof(JValue).IsAssignableFrom(type): return "Edm.PrimitiveType";
                        case var _ when typeof(JToken).IsAssignableFrom(type): return "Edm.ComplexType";
                        case var _ when type.Implements(typeof(IEnumerable<>), out var p): return $"Collection({GetEdmTypeName(p[0])})";
                        default: return $"global.{type.FullName}";
                    }
                case TypeCode.Boolean: return "Edm.Boolean";
                case TypeCode.Byte: return "Edm.Byte";
                case TypeCode.DateTime: return "Edm.DateTimeOffset";
                case TypeCode.Decimal: return "Edm.Decimal";
                case TypeCode.Double: return "Edm.Double";
                case TypeCode.Single: return "Edm.Single";
                case TypeCode.Int16: return "Edm.Int16";
                case TypeCode.UInt16:
                case TypeCode.Int32: return "Edm.Int32";
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int64: return "Edm.Int64";
                case TypeCode.SByte: return "Edm.SByte";
                case TypeCode.Char:
                case TypeCode.String: return "Edm.String";
                default: return "global." + type.FullName;
            }
        }
    }
}