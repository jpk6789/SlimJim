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
			//Assert.That(repository.Finder, Is.Not.Null, "Should have created instance of CsProjFinder.");
			//Assert.That(repository.Reader, Is.Not.Null, "Should have created instance of CsProjReader.");
		}

		[TestMethod]
		public void GetsFilesFromFinderAndProcessesThemWithCsProjReader()
		{
			finder.Expect(f => f.FindAllProjectFiles(options.ProjectTypes,WorkingDirectory)).Return(new List<FileInfo>{file1, file2});
			reader.Expect(r => r.Read(file1)).Return(proj1);
			reader.Expect(r => r.Read(file2)).Return(proj2);

			List<CsProj> projects = repository.LookupCsProjsFromDirectory(options);

			CollectionAssert.AreEqual(new[] { proj1, proj2 }, projects);
			//Assert.That(projects, Is.EqualTo(new[]{proj1, proj2}));
		}

		[TestMethod]
		public void GracefullyHandlesNullsFromReader()
		{
			finder.Expect(f => f.FindAllProjectFiles(options.ProjectTypes, WorkingDirectory)).Return(new List<FileInfo> { file1, file2 });
			reader.Expect(r => r.Read(file1)).Return(proj1);
			reader.Expect(r => r.Read(file2)).Return(null);

			List<CsProj> projects = repository.LookupCsProjsFromDirectory(options);

			CollectionAssert.AreEqual(new[] { proj1 }, projects);
			//Assert.That(projects, Is.EqualTo(new[] { proj1 }));
		}

		[TestMethod]
		public void ReadsFilesFromAdditionalSearchPathsAsWell()
		{
			options.AddAdditionalSearchPaths(new[] { SearchPath1, SearchPath2 });
			finder.Expect(f => f.FindAllProjectFiles(options.ProjectTypes, WorkingDirectory)).Return(new List<FileInfo>());
			finder.Expect(f => f.FindAllProjectFiles(options.ProjectTypes, SearchPath1)).Return(new List<FileInfo>());
			finder.Expect(f => f.FindAllProjectFiles(options.ProjectTypes, SearchPath2)).Return(new List<FileInfo>());

			repository.LookupCsProjsFromDirectory(options);
		}

		[TestMethod]
		public void IngoresDirectoryPatternsInOptions()
		{
			options.AddIgnoreDirectoryPatterns("Folder1", "Folder2");
			finder.Expect(f => f.IgnorePatterns(new[] {"Folder1", "Folder2"}));
			finder.Expect(f => f.FindAllProjectFiles(options.ProjectTypes, WorkingDirectory)).Return(new List<FileInfo>());

			repository.LookupCsProjsFromDirectory(options);
		}
	}
}