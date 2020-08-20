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
		}

		[TestMethod]
		public void SolutionOutputPathUsesGivenValueIfSet()
		{
			string slnOutputPath = GetSamplePath("Projects", "Solutions");
			options = new SlnGenerationOptions(GetSamplePath("Projects")) {SlnOutputPath = slnOutputPath};

			StringAssert.Equals(slnOutputPath, options.SlnOutputPath);
		}

		[TestMethod]
		public void UnspecifiedSolutionNameWithNoTargetProjectsUsesFolderName()
		{
			options = new SlnGenerationOptions(WorkingDirectory);
			StringAssert.Equals("WorkingDir", options.SolutionName);

			options = new SlnGenerationOptions(Path.Combine(WorkingDirectory, "SlumJim"));
			StringAssert.Equals("SlumJim", options.SolutionName);

			options = new SlnGenerationOptions(Path.Combine(WorkingDirectory, "SlumJim") + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar);
			StringAssert.Equals("SlumJim", options.SolutionName);

			options = new SlnGenerationOptions(Path.DirectorySeparatorChar.ToString());
			StringAssert.Equals("SlimJim", options.SolutionName);
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
		}

		[TestMethod]
		public void RelativeSlnOutputPathRootedAtProjectsRoot()
		{
			var root = GetSamplePath ("Proj", "Root");
			options = new SlnGenerationOptions (root);
			options.SlnOutputPath = "Solutions";

			StringAssert.Equals(Path.Combine(root, "Solutions"), options.SlnOutputPath);
		}

		[TestMethod]
		public void RelativeProjectsRootDirIsRootedAtWorkingDir()
		{
			options = new SlnGenerationOptions(WorkingDirectory);
			options.ProjectsRootDirectory = Path.Combine("Proj", "Root");

			StringAssert.Equals(Path.Combine(WorkingDirectory, "Proj", "Root"), options.SlnOutputPath);
		}
	}
}