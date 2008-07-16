//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Habanero.BO.Loaders {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Dtds {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Dtds() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Habanero.BO.Loaders.Dtds", typeof(Dtds).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include key.dtd
        ///#include primaryKey.dtd
        ///#include property.dtd
        ///#include relationship.dtd
        ///#include superClass.dtd
        ///#include ui.dtd
        ///&lt;!ELEMENT class (superClass?, property*, key*, primaryKey?, relationship*, ui*)&gt;
        ///&lt;!ATTLIST class
        ///		name NMTOKEN #REQUIRED
        ///		assembly NMTOKEN #REQUIRED
        ///		table CDATA #IMPLIED
        ///		displayName CDATA #IMPLIED
        ///&gt;
        ///
        ///.
        /// </summary>
        internal static string _class {
            get {
                return ResourceManager.GetString("_class", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT businessObjectLookupList EMPTY&gt;
        ///&lt;!ATTLIST businessObjectLookupList 
        ///	class NMTOKEN #REQUIRED
        ///	assembly NMTOKEN #REQUIRED
        ///  criteria CDATA #IMPLIED
        ///  sort CDATA #IMPLIED
        ///&gt;.
        /// </summary>
        internal static string businessObjectLookupList {
            get {
                return ResourceManager.GetString("businessObjectLookupList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include class.dtd
        ///&lt;!ELEMENT classes (class+)&gt;
        ///.
        /// </summary>
        internal static string classes {
            get {
                return ResourceManager.GetString("classes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include parameter.dtd
        ///&lt;!ELEMENT column (parameter*)&gt;
        ///&lt;!ATTLIST column
        ///	heading CDATA #IMPLIED
        ///	property CDATA #REQUIRED
        ///	type NMTOKEN	#IMPLIED
        ///	assembly NMTOKEN #IMPLIED
        ///	editable ( true | false ) &quot;true&quot;
        ///	width CDATA &quot;100&quot;
        ///	alignment ( left | right | center | centre ) &quot;left&quot;
        ///&gt;.
        /// </summary>
        internal static string column {
            get {
                return ResourceManager.GetString("column", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include field.dtd
        ///&lt;!ELEMENT columnLayout (field+)&gt;
        ///&lt;!ATTLIST columnLayout
        ///	width CDATA &quot;-1&quot;
        ///&gt;
        ///.
        /// </summary>
        internal static string columnLayout {
            get {
                return ResourceManager.GetString("columnLayout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT databaseLookupList EMPTY&gt;
        ///&lt;!ATTLIST databaseLookupList
        ///	sql CDATA #REQUIRED
        ///  timeout CDATA &quot;10000&quot;
        ///	class NMTOKEN #IMPLIED
        ///	assembly NMTOKEN #IMPLIED
        ///&gt;.
        /// </summary>
        internal static string databaseLookupList {
            get {
                return ResourceManager.GetString("databaseLookupList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include parameter.dtd
        ///#include trigger.dtd
        ///&lt;!ELEMENT field (parameter*, trigger*)&gt;
        ///&lt;!ATTLIST field
        ///	label CDATA #IMPLIED
        ///	property NMTOKEN #REQUIRED
        ///	type NMTOKEN #IMPLIED
        ///	assembly NMTOKEN #IMPLIED
        ///	mapperType NMTOKEN #IMPLIED
        ///  mapperAssembly NMTOKEN #IMPLIED
        ///  editable ( true | false ) &quot;true&quot;
        ///	toolTipText CDATA #IMPLIED
        ///&gt;.
        /// </summary>
        internal static string field {
            get {
                return ResourceManager.GetString("field", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include tab.dtd
        ///#include columnLayout.dtd
        ///#include field.dtd
        ///&lt;!ELEMENT form (tab*, columnLayout*, field*)&gt;
        ///&lt;!ATTLIST form
        ///	width CDATA &quot;300&quot;
        ///	height CDATA &quot;250&quot;
        ///	title CDATA &quot;&quot;
        ///&gt;
        ///.
        /// </summary>
        internal static string form {
            get {
                return ResourceManager.GetString("form", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT formGrid EMPTY&gt;
        ///&lt;!ATTLIST formGrid
        ///	relationship NMTOKEN #REQUIRED
        ///	reverseRelationship NMTOKEN #REQUIRED
        ///    type NMTOKEN &quot;Habanero.UI.Grid.EditableGrid&quot;
        ///	assembly NMTOKEN &quot;Habanero.UI&quot;
        ///&gt;.
        /// </summary>
        internal static string formGrid {
            get {
                return ResourceManager.GetString("formGrid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include column.dtd
        ///&lt;!ELEMENT grid (column+)&gt;
        ///&lt;!ATTLIST grid
        ///	sortColumn CDATA &quot;&quot;
        ///&gt;.
        /// </summary>
        internal static string grid {
            get {
                return ResourceManager.GetString("grid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include Prop.dtd
        ///&lt;!ELEMENT key (prop+)&gt;
        ///&lt;!ATTLIST key 
        ///	name CDATA #IMPLIED
        ///	ignoreIfNull ( true | false ) &quot;false&quot;
        ///  message CDATA #IMPLIED
        ///&gt;
        ///.
        /// </summary>
        internal static string key {
            get {
                return ResourceManager.GetString("key", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT parameter EMPTY&gt;
        ///&lt;!ATTLIST parameter
        ///	name NMTOKEN #REQUIRED
        ///	value CDATA #REQUIRED
        ///&gt;.
        /// </summary>
        internal static string parameter {
            get {
                return ResourceManager.GetString("parameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include Prop.dtd
        ///&lt;!ELEMENT primaryKey (prop+)&gt;
        ///&lt;!ATTLIST primaryKey 
        ///	isObjectID ( true | false ) &quot;true&quot;
        ///&gt;
        ///.
        /// </summary>
        internal static string primaryKey {
            get {
                return ResourceManager.GetString("primaryKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT prop EMPTY&gt;
        ///&lt;!ATTLIST prop
        ///	name NMTOKEN #REQUIRED
        ///&gt;.
        /// </summary>
        internal static string Prop {
            get {
                return ResourceManager.GetString("Prop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include rule.dtd
        ///#include databaseLookupList.dtd
        ///#include simpleLookupList.dtd
        ///#include businessObjectLookupList.dtd
        ///&lt;!ELEMENT property (rule*, databaseLookupList*, simpleLookupList*, businessObjectLookupList*)&gt;
        ///&lt;!ATTLIST property
        ///	name NMTOKEN #REQUIRED
        ///	displayName CDATA #IMPLIED
        ///	type CDATA &quot;String&quot;
        ///	assembly NMTOKEN &quot;System&quot;
        ///	readWriteRule (	ReadWrite | ReadOnly | WriteOnce | WriteNotNew | WriteNew ) &quot;ReadWrite&quot;
        ///	databaseField CDATA #IMPLIED
        ///	default CDATA #IMPLIED
        ///	description CDATA #IMPL [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string property {
            get {
                return ResourceManager.GetString("property", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT relationship (relatedProperty+)&gt;
        ///&lt;!ATTLIST relationship
        ///	name NMTOKEN #REQUIRED
        ///	type (single | multiple) #REQUIRED
        ///	relatedClass NMTOKEN #REQUIRED
        ///	relatedAssembly	NMTOKEN #REQUIRED
        ///	keepReference ( true | false ) &quot;true&quot;
        ///	orderBy CDATA &quot;&quot;
        ///	deleteAction ( DeleteRelated | DereferenceRelated | Prevent | DoNothing ) &quot;Prevent&quot;
        ///&gt;
        ///		&lt;!ELEMENT relatedProperty EMPTY&gt;
        ///    &lt;!ATTLIST relatedProperty
        ///			property NMTOKEN #REQUIRED
        ///			relatedProperty NMTOKEN #REQUIRED
        ///		&gt;.
        /// </summary>
        internal static string relationship {
            get {
                return ResourceManager.GetString("relationship", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT add EMPTY&gt;
        ///&lt;!ATTLIST add
        ///	key NMTOKEN #REQUIRED
        ///  value CDATA #REQUIRED
        ///&gt;
        ///&lt;!ELEMENT rule (add*)&gt;
        ///&lt;!ATTLIST rule
        ///	name CDATA #REQUIRED
        ///	class NMTOKEN #IMPLIED
        ///  assembly NMTOKEN #IMPLIED
        ///  message CDATA #IMPLIED
        ///&gt;.
        /// </summary>
        internal static string Rule {
            get {
                return ResourceManager.GetString("Rule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT simpleLookupList (item*)&gt;
        ///&lt;!ATTLIST simpleLookupList
        ///  options CDATA #IMPLIED
        ///  &gt;
        ///&lt;!ELEMENT item EMPTY&gt;
        ///&lt;!ATTLIST item
        ///	display CDATA #REQUIRED
        ///	value   CDATA #REQUIRED
        ///&gt;
        ///.
        /// </summary>
        internal static string simpleLookupList {
            get {
                return ResourceManager.GetString("simpleLookupList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT superClass EMPTY&gt;
        ///&lt;!ATTLIST superClass
        ///		class NMTOKEN #REQUIRED
        ///		assembly NMTOKEN #REQUIRED
        ///		orMapping ( ClassTableInheritance | SingleTableInheritance | ConcreteTableInheritance ) &quot;ClassTableInheritance&quot;
        ///    id CDATA #IMPLIED
        ///    discriminator CDATA #IMPLIED
        ///&gt;.
        /// </summary>
        internal static string superClass {
            get {
                return ResourceManager.GetString("superClass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include columnLayout.dtd
        ///#include formGrid.dtd
        ///#include field.dtd
        ///&lt;!ELEMENT tab (columnLayout*, field*, formGrid?)&gt;
        ///&lt;!ATTLIST tab
        ///	name CDATA #REQUIRED
        ///&gt;
        ///.
        /// </summary>
        internal static string tab {
            get {
                return ResourceManager.GetString("tab", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!ELEMENT trigger EMPTY&gt;
        ///&lt;!ATTLIST trigger
        ///	triggeredBy NMTOKEN #IMPLIED
        ///	conditionValue CDATA #IMPLIED
        ///	action NMTOKEN #REQUIRED
        ///	value CDATA #IMPLIED
        ///  target NMTOKEN #IMPLIED
        ///&gt;.
        /// </summary>
        internal static string trigger {
            get {
                return ResourceManager.GetString("trigger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include grid.dtd
        ///#include form.dtd
        ///&lt;!ELEMENT ui (grid?, form?)&gt;
        ///&lt;!ATTLIST ui
        ///	name NMTOKEN &quot;default&quot;
        ///&gt;.
        /// </summary>
        internal static string ui {
            get {
                return ResourceManager.GetString("ui", resourceCulture);
            }
        }
    }
}
