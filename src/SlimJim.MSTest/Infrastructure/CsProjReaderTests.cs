using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimJim.Infrastructure;
using SlimJim.Model;
using System.Collections.Generic;

namespace SlimJim.Test.Infrastructure
{
	[TestClass]
	public class CsProjReaderTests
	{
		private FileInfo file;

		[TestMethod]
		public void ReadsFileContentsIntoObject()
		{
			CsProj project = GetProject("Simple");


			Assert.AreEqual("{4A37C916-5AA3-4C12-B7A8-E5F878A5CDBA}", project.Guid);
			Assert.AreEqual("MyProject", project.AssemblyName);
			Assert.AreEqual(file.FullName, project.Path);
			Assert.AreEqual("v4.0", project.TargetFrameworkVersion);
			CollectionAssert.AreEqual(new[]
										{
											"System",
											"System.Core",
											"System.Xml.Linq",
											"System.Data.DataSetExtensions",
											"Microsoft.CSharp",
											"System.Data",
											"System.Xml"
										}, project.ReferencedAssemblyNames);
			CollectionAssert.AreEqual(new[]
										{
											"{99036BB6-4F97-4FCC-AF6C-0345A5089099}",
											"{69036BB3-4F97-4F9C-AF2C-0349A5049060}"
										}, project.ReferencedProjectGuids);
		}

		[TestMethod]
		public void IgnoresNestedReferences()
		{
			CsProj project = GetProject("ConvertedReference");


			CollectionAssert.DoesNotContain(project.ReferencedAssemblyNames, "log4net");
		}

		[TestMethod]
		public void TakesOnlyNameOfFullyQualifiedAssemblyName()
		{
			CsProj project = GetProject("FQAssemblyName");

			CollectionAssert.Contains(project.ReferencedAssemblyNames, "NHibernate");
		}

		[TestMethod]
		public void NoProjectReferencesDoesNotCauseNRE()
		{
			CsProj project = GetProject("NoProjectReferences");

			CollectionAssert.AreEqual(new List<string>(), project.ReferencedProjectGuids);
		}

		[TestMethod]
		public void NoAssemblyName_ReturnsNull()
		{
			CsProj project = GetProject("BreaksThings");

			Assert.IsNull(project);
		}

		private CsProj GetProject(string fileName)
		{
			file = SampleFiles.SampleFileHelper.GetCsProjFile(fileName);
			var reader = new CsProjReader();
			return reader.Read(file);
		}
	}
}