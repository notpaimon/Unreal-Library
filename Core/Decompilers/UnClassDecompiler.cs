﻿#if DECOMPILE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UELib;
using UELib.Core;

namespace UELib.Core
{
	public partial class UClass : UState
	{
		/**
		 * Structure looks like this, even though said XX.GetFriendlyName() actually it's XX.Decompile() which handles then the rest on its own.
		 * class GetName() extends SuperFieldName
		 * 		FormatFlags()
		 * 		
		 * Constants
		 * const C.GetFriendlyName() = C.Value
		 * 
		 * Enums
		 * enum En.GetFriendlyName()
		 * {
		 * 		FormatProperties()		
		 * }
		 * 
		 * Structs
		 * struct FormatFlags() Str.GetFriendlyName() extends SuperFieldName
		 * {
		 * 		FormatProperties()
		 * }
		 * 
		 * Properties
		 * var(GetCategoryName) FormatFlags() Prop.GetFriendlyName()
		 * 
		 * Replication
		 * {
		 * 		SerializeToken()
		 * }
		 * 
		 * Functions
		 * FormatFlags() GetFriendlyName() GetParms()
		 * {
		 * 		FormatLocals()
		 * 		SerializeToken()
		 * }
		 * 
		 * States
		 * FormatFlags() state GetFriendlyName() extends SuperFieldName
		 * {
		 * 		FormatIgnores()
		 * 		FormatFunctions()
		 * 		SerializeToken();
		 * }
		 * 
		 * DefaultProperties
		 * {
		 * }
		 */
		public override string Decompile()
		{
			string content = 
				"/*******************************************************************************" +
				"\r\n * Decompiled by " + System.Windows.Forms.Application.ProductName + ", an application developed by Eliot van Uytfanghe!" +
				"\r\n * Path " + System.IO.Path.GetFileNameWithoutExtension( Package.FullPackageName ) + "\\Classes\\" + Name + ".uc" +
				//"\r\n * " + GetDependencies() +
				"\r\n * " + GetImports() +
				"\r\n * " + GetStats() +
				"\r\n *******************************************************************************/\r\n";

			content += FormatHeader() +
				FormatConstants() +
				FormatEnums() +
				FormatStructs() +
				FormatProperties() +
				FormatReplication() +
				FormatFunctions() +
				FormatStates() +
				FormatDefaultProperties();	

			return content;
		}

		public string GetDependencies()
		{
			if( ClassDependenciesList == null )
				return String.Empty;

			string output = String.Empty;
			foreach( Dependency Dep in ClassDependenciesList )
			{
				UObject Obj = GetIndexObject( Dep.Class );
				if( Obj != null )
				{
					output += " *\t" + Obj.GetClassName() + " " + Obj.GetOuterGroup() + "\r\n";
				}
			}
			return output.Length != 0 ? "Class Dependencies:\r\n" + output + " *" : String.Empty;
		}

		public string GetImports()
		{
			if( PackageImportsList == null )
				return String.Empty;

			string output = String.Empty;
			for( var i = 0; i < PackageImportsList.Count; i ++ )
			{
				output += " *\t" + Package.NameTableList[PackageImportsList[i]].Name + "\r\n";
				/*for( int j = 1; j < (i + i) && (j + (i + i)) < PackageImportsList.Count; ++ j )
				{
					Output += " *\t\t\t" + Owner.NameTableList[PackageImportsList[i + j]].Name + "\r\n";
				}
				i += i;*/
			}
			return output.Length != 0 ? "Package Imports:\r\n" + output + " *" : String.Empty;
		}
														   
		public string GetStats()
		{
			string output = String.Empty;

			if( _ChildConstants != null && _ChildConstants.Count > 0 )
				output += " *\tConstants:" + _ChildConstants.Count + "\r\n";

			if( _ChildEnums != null && _ChildEnums.Count > 0 )
				output += " *\tEnums:" + _ChildEnums.Count + "\r\n";

			if( _ChildStructs != null && _ChildStructs.Count > 0 )
				output += " *\tStructs:" + _ChildStructs.Count + "\r\n";

			if( _ChildProperties != null && _ChildProperties.Count > 0 )
				output += " *\tProperties:" + _ChildProperties.Count + "\r\n";

			if( _ChildFunctions != null && _ChildFunctions.Count > 0 )
				output += " *\tFunctions:" + _ChildFunctions.Count + "\r\n";

			if( _ChildStates != null && _ChildStates.Count > 0 )
				output += " *\tStates:" + _ChildStates.Count + "\r\n";

			return output.Length != 0 ? "Stats:\r\n" + output + " *" : String.Empty;
		}

		protected override string FormatHeader()
		{
			string output = (
				Super != null && String.Compare( Super.Name, "Interface", true ) == 0 
				? "interface " 
				: "class ") 
				+ Name;

			// Object doesn't have an extension so only try add the extension if theres a SuperField
			if( Super != null )
			{
				output += " " + FormatExtends() + " " + Super.Name;
			}

			// Check within because within is Object by default
			if( _Within != null && !String.Equals( _Within.Name, "Object", StringComparison.OrdinalIgnoreCase ) )
			{
				output += " within " + _Within.Name;
			}

			string rules = FormatFlags();
			return output + (String.IsNullOrEmpty( rules ) ? ";" : rules);
		}

		private string FormatNameGroup( string groupName, IList<int> enumerableList )
		{
			string output = String.Empty;
			if( enumerableList != null && enumerableList.Count() > 0 )
			{
				output += "\r\n\t" + groupName + "(";
				try
				{
					foreach( int index in enumerableList )
					{
						output += Package.NameTableList[index].Name + ",";	
					}
					output = output.TrimEnd( ',' ) + ")";
				}
				catch
				{
					output += "\r\n\t/* An exception occurred while decompiling " + groupName + ". */";
				}
			}
			return output;
		}

		private string FormatObjectGroup( string groupName, IList<int> enumerableList )
		{
			string output = String.Empty;
			if( enumerableList != null && enumerableList.Count() > 0 )
			{
				output += "\r\n\t" + groupName + "(";
				try
				{
					foreach( int index in enumerableList )
					{
						output += Package.GetIndexObjectName( index ) + ",";	
					}
					output = output.TrimEnd( ',' ) + ")";
				}
				catch
				{
					output += "\r\n\t/* An exception occurred while decompiling " + groupName + ". */";
				}
			}
			return output;
		}

		private string FormatFlags()
		{
			string output = String.Empty;

			try{if( Package.Version >= UnrealPackage.VDLLBind && _DLLNameIndex != 0 && String.Compare( DLLName, "None", true ) != 0 )
			{output += "\r\n\tdllbind(" + DLLName + ")";}}catch{}

			if( ClassDependenciesList != null )
			{
				var dependson = new List<int>();
				for( int i = 0; i < ClassDependenciesList.Count; ++ i )
				{
					if( dependson.Exists( dep => dep == ClassDependenciesList[i].Class ) )
					{
						continue;
					}
					UClass obj = (UClass)GetIndexObject( ClassDependenciesList[i].Class );
					// Only exports and those who are further than this class
					if( obj != null && obj.ExportIndex > ExportIndex )
					{
						output += "\r\n\tdependson(" + obj.Name + ")";
					}
					dependson.Add( ClassDependenciesList[i].Class );
				}
			}

			output += FormatNameGroup( "dontsortcategories", DontSortCategoriesList );
			output += FormatNameGroup( "hidecategories", HideCategoriesList );
			output += FormatNameGroup( "classgroup", ClassGroupsList );
			output += FormatNameGroup( "autoexpandcategories", AutoExpandCategoriesList );
			output += FormatNameGroup( "autocollapsecategories", AutoCollapseCategoriesList );
			output += FormatObjectGroup( "implements", ImplementedInterfacesList );

			if( HasObjectFlag( Flags.ObjectFlagsLO.Native ) )
			{
				output += "\r\n\t" + FormatNative();
				if( NativeClassName.Length != 0 )
				{
					output += "(" + NativeClassName + ")";
				}
			}

			if( HasClassFlag( Flags.ClassFlags.NativeReplication ) )
			{
				output += "\r\n\tnativereplication";
			}
			/*else
			{
				// Only do if parent had NativeReplication
				UClass parentClass = (UClass)Super;
				if( parentClass != null && parentClass.HasClassFlag( Flags.ClassFlags.NativeReplication ) )
				{
					output += "\r\n\tnonativereplication";
				}
			}*/

 			// BTClient.Menu.uc has Config(ClientBtimes) and this flag is not true???
			if( (ClassFlags & (uint)Flags.ClassFlags.Config) != 0 )
			{
				string inner = ConfigName;
				if( String.Compare( inner, "None", true ) == 0 || String.Compare( inner, "System", true ) == 0 )
				{
					inner = String.Empty;
				}
				output += "\r\n\tconfig(" + inner + ")"; //" /* File:" + System.IO.Path.GetDirectoryName( Package.FullPackageName ) 
					//+ "\\" + (inner == String.Empty ? Name : inner) + ".ini */";
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.ParseConfig) != 0 )
			{
				output += "\r\n\tparseconfig";
			}

			// Move to Header
			/*if( (ClassFlags & (uint)Unreal_Explorer.ClassFlags.Localized) != 0 )
			{
				Output += "\t\n\tlocalized";
			}*/

			if( (ClassFlags & (uint)Flags.ClassFlags.Transient) != 0 )
			{
				output += "\r\n\ttransient";
			}
			else
			{
				// Only do if parent had Transient
				UClass parentClass = (UClass)Super;
				if( parentClass != null && (parentClass.ClassFlags & (uint)Flags.ClassFlags.Transient) != 0 )
				{
					output += "\r\n\tnotransient";
				}
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.PerObjectConfig) != 0 )
			{
				output += "\r\n\tperobjectconfig";
			}
			else
			{
				// Only do if parent had PerObjectConfig
				UClass parentClass = (UClass)Super;
				if( parentClass != null && (parentClass.ClassFlags & (uint)Flags.ClassFlags.PerObjectConfig) != 0 )
				{
					output += "\r\n\tnoperobjectconfig";
				}
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.EditInlineNew) != 0 )
			{
				output += "\r\n\teditinlinenew";
			}
			else
			{
				// Only do if parent had EditInlineNew
				UClass parentClass = (UClass)Super;
				if( parentClass != null && (parentClass.ClassFlags & (uint)Flags.ClassFlags.EditInlineNew) != 0 )
				{
					output += "\r\n\tnoeditinlinenew";
				}
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.CollapseCategories) != 0 )
			{
				output += "\r\n\tcollapsecategories";
			}

			// FIX: Might indicate "Interface" in later versions
			if( HasClassFlag( Flags.ClassFlags.ExportStructs ) && Package.Version < 300 )
			{
				output += "\r\n\texportstructs";
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.NoExport) != 0 )
			{
				output += "\r\n\tnoexport";
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.Abstract) != 0 )
			{
				output += "\r\n\tabstract";
			}

			if( Extends( "Actor" ) )
			{
				if( (ClassFlags & (uint)Flags.ClassFlags.Placeable) != 0 )
				{
					output += Package.Version > 120 ? "\r\n\tplaceable" : "\r\n\tusercreate";
				}
				else
				{
					// Only do if parent had placeable
					/*UClass ParentClass = (UClass)Super;
					if( ParentClass != null && (ParentClass.ClassFlags & (uint)Flags.ClassFlags.Placeable) != 0 )
					{*/
					output += Package.Version > 120 ? "\r\n\tnotplaceable" : "\r\n\tnousercreate";
					//}
				}
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.SafeReplace) != 0 )
			{
				output += "\r\n\tsafereplace";
			}

			// Approx version
			if( (ClassFlags & (uint)Flags.ClassFlags.Instanced) != 0 && Package.Version < 150 )
			{
				output += "\r\n\tinstanced";
			}

			if( (ClassFlags & (uint)Flags.ClassFlags.HideDropDown) != 0 )
			{
				output += "\r\n\thidedropdown";
			}

			if( Package.LicenseeVersion == (ushort)UnrealPackage.LicenseeVersions.UT2k4 )
			{
				if( HasClassFlag( Flags.ClassFlags.CacheExempt ) )
				{
					output += "\r\n\tcacheexempt";
				}
			}

			//Output += "\n\tguid(" + ClassGuid + ")";
			return output + ";\r\n";
		}

		private sealed class ReplicatedObject
		{
			public string Name = String.Empty;
			public ushort Offset;
			public bool Reliable = true;
		}

		public const byte ObjectsPerLine = 2;
		public string FormatReplication()
		{
			if( _ScriptSize <= 0 )
			{
				return String.Empty;
			}

			var replicationList = new List<ReplicatedObject>();
			if( _ChildProperties != null )
			{
				foreach( var prop in _ChildProperties )
				{
					if( (prop.PropertyFlags & (uint)Flags.PropertyFlagsLO.Net) != 0 )
					{
						// Could be a replicated property from parent class.
						if( prop.RepOffset > (ushort)_ScriptSize )
						{
							continue;
						}
						var ro = new ReplicatedObject {Name = prop.Name, Offset = Math.Min( prop.RepOffset, (ushort)_ScriptSize )};
						replicationList.Add( ro );
					}
				}
			}

			if( Package.Version < 189 && _ChildFunctions != null )
			{
				foreach( var func in _ChildFunctions )
				{
					if( (func.FunctionFlags & (uint)Flags.FunctionFlags.Net) != 0 )
					{
						// Could be a replicated function from parent class.
						if( func.RepOffset > (ushort)_ScriptSize )
						{
							continue;
						}
						var ro = new ReplicatedObject
						{
						    Name = func.Name,
						    Offset = func.RepOffset,
						    Reliable = ((func.FunctionFlags & (uint)Flags.FunctionFlags.NetReliable) != 0)
						};
						replicationList.Add( ro );
					}
				}
			}

			if( replicationList.Count == 0 )
			{
				return String.Empty;
			}

			// Must be sorted by Offset so that the buffer can read the statements linear!
			replicationList.Sort( (ro, ro2) => ro.Offset.CompareTo( ro2.Offset ) );

			// Important!
			replicationList.Reverse();

			string output = String.Empty;

			// Construct all replication blocks e.g. reliable(s) and unreliable(s).
			int num = 0;
			var codeDec = ByteCodeManager;
			codeDec.Deserialize();
			codeDec.InitDecompile();
			while( replicationList.Count > 0 )
			{
				bool bReliableState = replicationList[replicationList.Count - 1].Reliable;
				// End offset of this statement
				ushort offset = replicationList[replicationList.Count - 1].Offset;		
				// Add all replicated objects belonging to this replication statement
				//int IterationCount = 0;		
				var replicatedObjects = new List<string>();
				while( replicationList.Count > 0 )
				{
					// Print all replicated objects that are only in the same block(offset)
					if( replicationList[replicationList.Count - 1].Offset != offset )
					{
						// End of this Statement, print output and move to the next statement if any.
						break;
					}
					replicatedObjects.Add( replicationList[replicationList.Count - 1].Name );
					//output += "\r\n\t\tRemoved:" + replicationList[replicationList.Count - 1].Name;
					replicationList.Remove( replicationList[replicationList.Count - 1] );
				}

				/*{int i = 0;
				output += "\r\n\t\tOffset:" + offset;
				while( i < replicationList.Count )
				{
					output += "\r\n\t\tRemaining:" + replicationList[i].Name;
					++ i;
				}}*/

				string builtObjects = String.Empty;
				if( replicatedObjects.Count > 0 )
				{	
					for( int i = 0; i < replicatedObjects.Count; ++ i )
					{
						// Write two replicated objects per line.
						builtObjects += ((i % ObjectsPerLine == 0) ? "\r\n\t\t" : " ") 
							+ replicatedObjects[i] 
							+ ((i != replicatedObjects.Count - 1) ? "," : ";");
					}
					replicatedObjects.Clear();
				}

				string conditions = String.Empty;
				//codeDec.Goto( offset );
				//var t = codeDec.CurrentToken;
				var t = codeDec.NextToken;
				try
				{
					conditions = t.Decompile();
				}
				catch
				{
					conditions = "/* Exception occurred while decompiling token:" + t.GetType().Name + "*/";
				}

				output += "\r\n\t// Replication block:" + ++ num + "\r\n\t" + 
					(Package.Version < 189 ? (bReliableState ? "reliable" : "unreliable") : String.Empty) + " if(" + conditions + ")" +
						builtObjects + "\r\n";
			}
			replicationList.Clear();
			return (output.Length != 0 ? ("\r\nreplication\r\n{" + output + "}\r\n") : String.Empty);
		}

		private string FormatStates()
		{
			if( _ChildStates == null )
				return String.Empty;

			string output = String.Empty;
			foreach( var st in _ChildStates )
			{
				// And add a empty line between all states!
				output += "\r\n" + st.Decompile() + (st != _ChildStates.Last() ? "\r\n" : String.Empty);
			}
			return output + (output.Length != 0 ? "\r\n" : String.Empty);
		}
	}
}
#endif