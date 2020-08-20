using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimJim.Infrastructure;
using SlimJim.Model;
using System.Collections.Generic;

namespace SlimJim.Test.Infrastructure
{
	[TestClass]
	public class ArgsOptionsBuilderTests : TestBase
	{
		private SlnGenerationOptions options;

		[TestMethod]
		public void TestDefaults()
		{
			options = ArgsOptionsBuilder.BuildOptions(new string[0], WorkingDirectory);

			Assert.AreEqual(WorkingDirectory, options.ProjectsRootDirectory, "ProjectsRootDirectory");
			CollectionAssert.AreEqual(new List<string>(), options.TargetProjectNames, "TargetProjectNames");
			Assert.AreEqual(SlnGenerationMode.FullGraph, options.Mode, "Mode");
			CollectionAssert.AreEqual(new List<string>(), options.AdditionalSearchPaths, "AdditionalSearchPaths");
			Assert.IsFalse(options.IncludeEfferentAssemblyReferences, "IncludeEfferentAssemblyReferences");
			Assert.IsFalse(options.ShowHelp, "ShowHelp");
			Assert.IsFalse(options.OpenInVisualStudio, "OpenInVisualStudio");
		}

		[TestMethod]
		public void SpecifiedProjectsRootDirectory()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--root", WorkingDirectory }, WorkingDirectory);

			Assert.AreEqual(WorkingDirectory, options.ProjectsRootDirectory);
		}

		[TestMethod]
		public void SpecifiedTargetProject()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--target", "MyProject" }, WorkingDirectory);

			CollectionAssert.AreEqual(new[] { "MyProject" }, options.TargetProjectNames);
			Assert.AreEqual("MyProject", options.SolutionName);
		}

		[TestMethod]
		public void SpecifiedMultipleTargetProjects()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--target", "MyProject", "--target", "YourProject" }, WorkingDirectory);

			CollectionAssert.AreEqual(new[] { "MyProject", "YourProject" }, options.TargetProjectNames);
			Assert.AreEqual("MyProject_YourProject", options.SolutionName);
		}

		[TestMethod]
		public void SpecifiedAdditionalSearchPaths()
		{
			var otherDir = GetSamplePath("OtherProjects");
			var moreProjects = GetSamplePath("MoreProjects");
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--search", otherDir, "--search", moreProjects }, WorkingDirectory);

			CollectionAssert.AreEqual(new[] { otherDir, moreProjects }, options.AdditionalSearchPaths);
		}

		[TestMethod]
		public void SpecifiedSlnOuputPath()
		{
			var slnDir = GetSamplePath(WorkingDirectory, "Sln");
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--out", slnDir }, WorkingDirectory);

			Assert.AreEqual(slnDir, options.SlnOutputPath);
		}

		[TestMethod]
		public void SpecifiedVisualStudioVersion2008()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--version", "2008" }, WorkingDirectory);

			Assert.AreEqual(VisualStudioVersion.VS2008, options.VisualStudioVersion);
		}

		[TestMethod]
		public void SpecifiedVisualStudioVersion2010()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--version", "2010" }, WorkingDirectory);

			Assert.AreEqual(VisualStudioVersion.VS2010, options.VisualStudioVersion);
		}

		[TestMethod]
		public void SpecifiedVisualStudioVersion2012()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--version", "2012" }, WorkingDirectory);

			Assert.AreEqual(VisualStudioVersion.VS2012, options.VisualStudioVersion);
		}

		[TestMethod]
		public void SpecifiedVisualStudioVersion2013()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--version", "2013" }, WorkingDirectory);

			Assert.AreEqual(VisualStudioVersion.VS2013, options.VisualStudioVersion);
		}

		[TestMethod]
		public void SpecifiedVisualStudioVersion2015()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--version", "2015" }, WorkingDirectory);

			Assert.AreEqual(VisualStudioVersion.VS2015, options.VisualStudioVersion);
		}

		[TestMethod]
		public void SpecifiedVisualStudioVersion2017()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--version", "2017" }, WorkingDirectory);

			Assert.AreEqual(VisualStudioVersion.VS2017, options.VisualStudioVersion);
		}

		[TestMethod]
		public void InvalidVisualStudioVersionNumber()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--version", "dumb" }, WorkingDirectory);

			Assert.AreEqual(VisualStudioVersion.VS2019, options.VisualStudioVersion);
		}

		[TestMethod]
		public void SpecifiedSolutionName()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--name", "MyProjects" }, WorkingDirectory);

			Assert.AreEqual("MyProjects", options.SolutionName);
		}

		[TestMethod]
		public void UnspecifiedSolutionNameWithSingleTargetProject()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--target", "MyProject" }, WorkingDirectory);
		}

		[TestMethod]
		public void UnspecifiedSolutionNameWithMultipleTargetProjectsUsesFirstProjectNamePlusSuffix()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--target", "MyProject", "--target", "YourProject" }, WorkingDirectory);
		}
		
		[TestMethod]
		public void IncludeEfferentAssemblyReferences()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { @"--all" }, WorkingDirectory);

			Assert.IsTrue(options.IncludeEfferentAssemblyReferences);
		}

		[TestMethod]
		public void IgnoresFolderNames()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] {"--ignore", "Folder1", "--ignore", "Folder2"}, WorkingDirectory);

			CollectionAssert.AreEqual(new[] { "Folder1", "Folder2" }, options.IgnoreDirectoryPatterns);
		}

		[TestMethod]
		public void ShowHelp()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] {"--help"}, WorkingDirectory);
			// check console output
		}

		[TestMethod]
		public void ShowHelpIsSetOnOptions()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--help" }, WorkingDirectory);

			Assert.IsTrue(options.ShowHelp, "ShowHelp");
		}

		[TestMethod]
		public void SpecifyOpenInVisualStudio()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "--open" }, WorkingDirectory);

			Assert.IsTrue(options.OpenInVisualStudio, "OpenInVisualStudio");
		}

		[TestMethod]
		public void DefaultThresholdIsInfo()
		{
			options = ArgsOptionsBuilder.BuildOptions(new string[0], WorkingDirectory);

			Assert.AreEqual(Level.Info, options.LoggingThreshold);
		}

		[TestMethod]
		public void ExtraVerbose()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "-vv" }, WorkingDirectory);

			Assert.AreEqual(Level.Trace, options.LoggingThreshold);
		}

		[TestMethod]
		public void Quiet()
		{
			options = ArgsOptionsBuilder.BuildOptions(new[] { "-q" }, WorkingDirectory);

			Assert.AreEqual(Level.Warn, options.LoggingThreshold);
		}
	}
}