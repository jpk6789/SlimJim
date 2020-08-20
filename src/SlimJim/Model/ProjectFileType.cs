using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlimJim.Model
{
	public sealed class ProjectFileType
	{
		//.csproj', '.vbproj', '.vcxproj'
		private static readonly ProjectFileType csproj = new ProjectFileType("C# Projects", ".csproj");
		private static readonly ProjectFileType vbproj = new ProjectFileType("VisualBasic Projects", ".vbproj");
		private static readonly ProjectFileType vcxproj = new ProjectFileType("C++ Projects", ".vcxproj");

		private ProjectFileType(string name, string fileExtention)
		{
			Name = name;
			FileExtention = fileExtention;
		}

		public static ProjectFileType CSPROJ
		{
			get { return csproj; }
		}

		public static ProjectFileType VBPROJ
		{
			get { return vbproj; }
		}

		public static ProjectFileType VCXPROJ
		{
			get { return vcxproj; }
		}

		public static ProjectFileType[] AllExtentions
		{
			get
			{
				return new[] { csproj, vbproj, vcxproj };
			}
		}

		public string Name { get; private set; }
		public string FileExtention { get; private set; }

		public static ProjectFileType ParseTypeString(string projectType)
		{
			return AllExtentions?.FirstOrDefault(v => projectType.Contains(v.FileExtention));
		}
	}
}
