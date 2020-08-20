using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimJim.Model;
using System.Collections.Generic;

namespace SlimJim.Test.Model
{
	[TestClass]
	public class SlnTests
	{
		[TestMethod]
		public void VersionDefaultsTo2017()
		{
			Assert.AreEqual(VisualStudioVersion.VS2017, new Sln("sln").Version);
			//Assert.That(new Sln("sln").Version, Is.EqualTo(VisualStudioVersion.VS2017));
		}

		[TestMethod]
		public void GuidFormatIncludesCurlyBraces()
		{
			StringAssert.StartsWith(new Sln("sample").Guid, "{");
			StringAssert.EndsWith(new Sln("sample").Guid, "}");
			//Assert.That(new Sln("sample").Guid, Does.StartWith("{"));
			//Assert.That(new Sln("sample").Guid, Does.EndWith("}"));
		}

		[TestMethod]
		public void CreatesNoSolutionFoldersForSimpleProjectStructure()
		{
			var sln = new Sln("sample") {ProjectsRootDirectory = "Fake/Example"};
			sln.AddProjects(new CsProj {Path = "Fake/Example/ProjectA/ProjectA.csproj"});
			sln.AddProjects(new CsProj {Path = "Fake/Example/ProjectB/ProjectB.csproj"});

			Assert.IsNull(sln.Folders, "Folders");
			//Assert.That(sln.Folders, Is.Null, "Folders");
		}

		[TestMethod]
		public void CreatesSolutionFoldersForNestedProjectStructure()
		{
			var sln = new Sln("sample") { ProjectsRootDirectory = "Fake/Example" };
			sln.AddProjects(new CsProj { Path = "Fake/Example/ModuleA/ProjectA/ProjectA.csproj" });
			sln.AddProjects(new CsProj { Path = "Fake/Example/ModuleA/ProjectB/ProjectB.csproj" });

			var folder = sln.Folders.FirstOrDefault();

			StringAssert.Equals("ModuleA", folder.FolderName);
			Assert.AreEqual(2, folder.ContentGuids.Count);
			//Assert.That(folder.FolderName, Is.EqualTo("ModuleA"));
			//Assert.That(folder.ContentGuids.Count, Is.EqualTo(2));
		}

		[TestMethod]
		public void CreatesNestedSolutionFolders()
		{
			var sln = new Sln("sample") { ProjectsRootDirectory = "Fake/Example" };
			var proj = new CsProj { Path = "Fake/Example/Grouping1/ModuleA/ProjectA/ProjectA.csproj", Guid = Guid.NewGuid().ToString("B")};
			sln.AddProjects(proj);

			var child = sln.Folders.First(f => f.FolderName == "ModuleA");
			var parent = sln.Folders.First(f => f.FolderName == "Grouping1");

			IList<string> test = new List<string>();

			Assert.AreEqual(new[] { child.Guid }, parent.ContentGuids);
			Assert.AreEqual(new[] { proj.Guid }, child.ContentGuids);
			//Assert.That(parent.ContentGuids, Is.EqualTo(new[] {child.Guid}));
			//Assert.That(child.ContentGuids, Is.EqualTo(new[] { proj.Guid }));
		}

		[TestMethod]
		public void HandlesTrailingSlashOnRootDirectory()
		{
			var sln = new Sln("sample") { ProjectsRootDirectory = "Fake/Example/" };
			var proj = new CsProj { Path = "Fake/Example/ModuleA/ProjectA/ProjectA.csproj", Guid = Guid.NewGuid().ToString("B") };
			sln.AddProjects(proj);

			Assert.AreNotEqual(new Folder(), sln.Folders.FirstOrDefault(f => f.FolderName == "ModuleA"), "Folders");
			//Assert.That(sln.Folders.FirstOrDefault(f => f.FolderName == "ModuleA"), Is.Not.Null, "Folders");
		}
	}
}