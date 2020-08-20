using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimJim.Model;
using System.IO;

namespace SlimJim.Test.Model
{
	[TestClass]
	public class SlnGenerationOptionsTests : TestBase
	{
		private SlnGenerationOptions options;

		[TestMethod]
		public void SolutionOutputPathDefaultsToProjectsRootPath()
		{
			options = new SlnGenerationOptions(GetSamplePath("Projects"));

			StringAssert.Equals(options.ProjectsRootDirectory, options.SlnOutputPath);
			//Assert.That(options.SlnOutputPath, Is.EqualTo(options.ProjectsRootDirectory));
		}

		[TestMethod]
		public void SolutionOutputPathUsesGivenValueIfSet()
		{
			string slnOutputPath = GetSamplePath("Projects", "Solutions");
			options = new SlnGenerationOptions(GetSamplePath("Projects")) {SlnOutputPath = slnOutputPath};

			StringAssert.Equals(slnOutputPath, options.SlnOutputPath);
			//Assert.That(options.SlnOutputPath, Is.EqualTo(slnOutputPath));
		}

		[TestMethod]
		public void UnspecifiedSolutionNameWithNoTargetProjectsUsesFolderName()
		{
			options = new SlnGenerationOptions(WorkingDirectory);
			StringAssert.Equals("WorkingDir", options.SolutionName);
			//Assert.That(options.SolutionName, Is.EqualTo("WorkingDir"));

			options = new SlnGenerationOptions(Path.Combine(WorkingDirectory, "SlumJim"));
			StringAssert.Equals("SlumJim", options.SolutionName);
			//Assert.That(options.SolutionName, Is.EqualTo("SlumJim"));

			options = new SlnGenerationOptions(Path.Combine(WorkingDirectory, "SlumJim") + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar);
			StringAssert.Equals("SlumJim", options.SolutionName);
			//Assert.That(options.SolutionName, Is.EqualTo("SlumJim"));

			options = new SlnGenerationOptions(Path.DirectorySeparatorChar.ToString());
			StringAssert.Equals("SlimJim", options.SolutionName);
			//Assert.That(options.SolutionName, Is.EqualTo("SlimJim"));
		}

		[TestMethod]
		public void AdditionalSearchPathsRootedAtProjectRoot()
		{
			var root = GetSamplePath ("Proj", "Root");
			options = new SlnGenerationOptions(root);
			var path1 = Path.Combine("..", "SearchPath");
			var path2 = Path.Combine("..", "..", "OtherPath", "Pork");
			options.AddAdditionalSearchPaths (path1, path2);

			CollectionAssert.AreEqual(new[] { Path.Combine(root, path1), Path.Combine(root, path2) }, options.AdditionalSearchPaths);
			//Assert.That(options.AdditionalSearchPaths, Is.EqualTo(new[] {Path.Combine(root, path1), Path.Combine(root, path2)}));
		}

		[TestMethod]
		public void RelativeSlnOutputPathRootedAtProjectsRoot()
		{
			var root = GetSamplePath ("Proj", "Root");
			options = new SlnGenerationOptions (root);
			options.SlnOutputPath = "Solutions";

			StringAssert.Equals(Path.Combine(root, "Solutions"), options.SlnOutputPath);
			//Assert.That(options.SlnOutputPath, Is.EqualTo(Path.Combine(root, "Solutions")));
		}

		[TestMethod]
		public void RelativeProjectsRootDirIsRootedAtWorkingDir()
		{
			options = new SlnGenerationOptions(WorkingDirectory);
			options.ProjectsRootDirectory = Path.Combine("Proj", "Root");

			StringAssert.Equals(Path.Combine(WorkingDirectory, "Proj", "Root"), options.SlnOutputPath);
			//Assert.That(options.SlnOutputPath, Is.EqualTo (Path.Combine (WorkingDirectory, "Proj", "Root")));
		}
	}
}