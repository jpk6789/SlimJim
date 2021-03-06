using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SlimJim.Infrastructure;
using SlimJim.Model;
using SlimJim.Test.SampleFiles;

namespace SlimJim.Test.Infrastructure
{
	[TestClass]
	public class CsProjRepositoryTests : TestBase
	{
		private string SearchPath1
		{
			get
			{
				return GetSamplePath("OtherProjects");
			}
		}

		private string SearchPath2
		{
			get
			{
				return GetSamplePath("MoreProjects");
			}
		}

		private readonly FileInfo file1 = SampleFileHelper.GetCsProjFile("Simple");
		private readonly FileInfo file2 = SampleFileHelper.GetCsProjFile("Simple");
		private readonly CsProj proj1 = new CsProj {AssemblyName = "Proj1"};
		private readonly CsProj proj2 = new CsProj {AssemblyName = "Proj1"};
		private ProjectFileFinder finder;
		private CsProjReader reader;
		private CsProjRepository repository;
		private SlnGenerationOptions options;

		[TestInitialize]
		public void BeforeEach()
		{
			options = new SlnGenerationOptions(WorkingDirectory);
			finder = MockRepository.GenerateStrictMock<ProjectFileFinder>();
			reader = MockRepository.GenerateStrictMock<CsProjReader>();
			repository = new CsProjRepository
			{
				Finder = finder,
				Reader = reader
			};
		}

		[TestCleanup]
		public void AfterEach()
		{
			finder.VerifyAllExpectations();
			reader.VerifyAllExpectations();
		}

		[TestMethod]
		public void CreatesOwnInstancesOfFinderAndReader()
		{
			repository = new CsProjRepository();

			Assert.IsNotNull(repository.Finder, "Should have created instance of CsProjFinder.");
			Assert.IsNotNull(repository.Reader, "Should have created instance of CsProjReader.");
		}

		[TestMethod]
		public void GetsFilesFromFinderAndProcessesThemWithCsProjReader()
		{
			if (options.ProjectTypes.Length > 0)
				finder.SetProjectTypes(options.ProjectTypes);

			finder.Expect(f => f.FindAllProjectFiles(WorkingDirectory)).Return(new List<FileInfo>{file1, file2});
			reader.Expect(r => r.Read(file1)).Return(proj1);
			reader.Expect(r => r.Read(file2)).Return(proj2);

			List<CsProj> projects = repository.LookupCsProjsFromDirectory(options);

			CollectionAssert.AreEqual(new[] { proj1, proj2 }, projects);
		}

		[TestMethod]
		public void GracefullyHandlesNullsFromReader()
		{
			if (options.ProjectTypes.Length > 0)
				finder.SetProjectTypes(options.ProjectTypes);

			finder.Expect(f => f.FindAllProjectFiles(WorkingDirectory)).Return(new List<FileInfo> { file1, file2 });
			reader.Expect(r => r.Read(file1)).Return(proj1);
			reader.Expect(r => r.Read(file2)).Return(null);

			List<CsProj> projects = repository.LookupCsProjsFromDirectory(options);

			CollectionAssert.AreEqual(new[] { proj1 }, projects);
		}

		[TestMethod]
		public void ReadsFilesFromAdditionalSearchPathsAsWell()
		{
			options.AddAdditionalSearchPaths(new[] { SearchPath1, SearchPath2 });

			if (options.ProjectTypes.Length > 0)
				finder.SetProjectTypes(options.ProjectTypes);

			finder.Expect(f => f.FindAllProjectFiles(WorkingDirectory)).Return(new List<FileInfo>());
			finder.Expect(f => f.FindAllProjectFiles(SearchPath1)).Return(new List<FileInfo>());
			finder.Expect(f => f.FindAllProjectFiles(SearchPath2)).Return(new List<FileInfo>());

			repository.LookupCsProjsFromDirectory(options);
		}

		[TestMethod]
		public void IngoresDirectoryPatternsInOptions()
		{
			options.AddIgnoreDirectoryPatterns("Folder1", "Folder2");

			if (options.ProjectTypes.Length > 0)
				finder.SetProjectTypes(options.ProjectTypes);

			finder.Expect(f => f.IgnorePatterns(new[] {"Folder1", "Folder2"}));
			finder.Expect(f => f.FindAllProjectFiles(WorkingDirectory)).Return(new List<FileInfo>());

			repository.LookupCsProjsFromDirectory(options);
		}
	}
}