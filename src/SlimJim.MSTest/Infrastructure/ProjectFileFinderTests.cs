using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimJim.Infrastructure;
using SlimJim.Model;
using SlimJim.Test.SampleFiles;
using System.Linq;

namespace SlimJim.Test.Infrastructure
{
	[TestClass]
	public class ProjectFileFinderTests : TestBase
	{
		private static readonly string SampleFileSystemPath = SampleFileHelper.GetSampleFileSystemPath();
		private ProjectFileFinder finder;
		private List<FileInfo> projectFiles;

		[TestInitialize]
		public void BeforeEach()
		{
			finder = new ProjectFileFinder();
		}

        [TestMethod]
        public void FindsOneProjectInFolderWithCsproj()
        {
            projectFiles = finder.FindAllProjectFiles(new[] { ProjectFileType.CSPROJ }, Path.Combine(SampleFileSystemPath, @"MyProject"));

			AssertFilesMatching(new[]
				{
					@"MyProject\MyProject.csproj"
				});
		}

		[TestMethod]
		public void ReturnsFileInfosForEachProjectInFileSystem()
		{
			projectFiles = finder.FindAllProjectFiles(new[] { ProjectFileType.CSPROJ }, SampleFileSystemPath);

			AssertFilesMatching(new[]
				{
					@"MyProject\MyProject.csproj",
					@"Ours\OurProject1\OurProject1.csproj",
					@"Ours\OurProject2\OurProject2.csproj",
					@"Theirs\TheirProject1\TheirProject1.csproj",
					@"Theirs\TheirProject2\TheirProject2.csproj",
					@"Theirs\TheirProject3\TheirProject3.csproj",
				});
		}

		[TestMethod]
		public void IgnoresRelativePath()
		{
			finder.IgnorePatterns("Their");
			projectFiles = finder.FindAllProjectFiles(new[] { ProjectFileType.CSPROJ }, SampleFileSystemPath);

			AssertFilesMatching(new[]
				{
					@"MyProject\MyProject.csproj",
					@"Ours\OurProject1\OurProject1.csproj",
					@"Ours\OurProject2\OurProject2.csproj"
				});
		}

		[TestMethod]
		public void IgnoresFileName()
		{
			finder.IgnorePatterns("TheirProject3.csproj");
			projectFiles = finder.FindAllProjectFiles(new[] { ProjectFileType.CSPROJ }, SampleFileSystemPath);

			AssertFilesMatching(new[]
				{
					@"MyProject\MyProject.csproj",
					@"Ours\OurProject1\OurProject1.csproj",
					@"Ours\OurProject2\OurProject2.csproj",
   					@"Theirs\TheirProject1\TheirProject1.csproj",
					@"Theirs\TheirProject2\TheirProject2.csproj",
				});
		}

		[TestMethod]
		public void IgnoresRelativePathWithDifferentCase()
		{
			finder.IgnorePatterns("ThEiR");
			projectFiles = finder.FindAllProjectFiles(new[] { ProjectFileType.CSPROJ }, SampleFileSystemPath);

			AssertFilesMatching(new[]
				{
					@"MyProject\MyProject.csproj",
					@"Ours\OurProject1\OurProject1.csproj",
					@"Ours\OurProject2\OurProject2.csproj"
				});
		}

		[TestMethod]
		public void IgnoresCertainFoldersByDefault()
		{
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, ".svn"))), ".svn folders ignored");
			Assert.IsFalse(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "doo.svn.wop"))), "don't ignore folders with .svn in the name");
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, ".hg"))), ".hg folders ignored");
			Assert.IsFalse(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "doo.hg.wop"))), "don't ignore folders with .hg in the name");
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, ".git"))), ".git folders ignored");
			Assert.IsFalse(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "doo.git.wop"))), "don't ignore folders with .git in the name");
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "bin"))), "bin folders ignored");
			Assert.IsFalse(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "obing"))), "don't ignore folders with bin in the name");
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "obj"))), "obj folders ignored");
			Assert.IsFalse(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "blobjee"))), "don't ignore folders with obj in the name");
		}

		[TestMethod]
		public void IgnoresReSharperFolders()
		{
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "_ReSharper.Something"))));
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "ReSharper"))));
			Assert.IsTrue(finder.PathIsIgnored(new DirectoryInfo(Path.Combine(WorkingDirectory, "___ReSharper___"))));
		}

		private void AssertFilesMatching(string[] expectedPaths)
		{
			expectedPaths = expectedPaths.Select(p => p.Replace ('\\', Path.DirectorySeparatorChar)).ToArray ();

			CollectionAssert.AreEqual(expectedPaths, projectFiles.ConvertAll(file => file.FullName.Replace(SampleFileSystemPath, "")));
		}
	}
}