using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;


namespace SlimJim.Test
{
    using SlimJim.Infrastructure;
    using SlimJim.Model;

    [TestClass]
	public class SlnFileGeneratorTests : TestBase
	{
		private const string TargetProject = "MyProject";
		private SlnFileGenerator gen;
		private SlnFileWriter slnWriter;
		private CsProjRepository repo;
		private SlnBuilder slnBuilder;
		private SlnGenerationOptions options;
		private readonly List<CsProj> projects = new List<CsProj>();
		private readonly Sln createdSlnObject = new Sln("Sln");

		private string ProjectsDir
		{
			get
			{
				return GetSamplePath ("Projects");
			}
		}
				
		[TestInitialize]
		public void BeforeEach()
		{
			repo = MockRepository.GenerateStrictMock<CsProjRepository>();
			slnWriter = MockRepository.GenerateStrictMock<SlnFileWriter>();
			slnBuilder = MockRepository.GenerateStrictMock<SlnBuilder>(new List<CsProj>());

			gen = new SlnFileGenerator()
			{
				ProjectRepository = repo,
				SlnWriter = slnWriter
			};

			SlnBuilder.OverrideDefaultBuilder(slnBuilder);
			options = new SlnGenerationOptions(ProjectsDir);
		}

		[TestMethod]
		public void CreatesOwnInstancesOfRepositoryAndWriter()
		{
			gen = new SlnFileGenerator();

			Assert.IsNotNull(gen.ProjectRepository, "Should have created instance of CsProjRepository.");
			Assert.IsNotNull(gen.SlnWriter, "Should have created instance of CsProjRepository.");
		}

		[TestMethod]
		public void GeneratesSlnFileForGivenOptions()
		{
			options.TargetProjectNames.Add(TargetProject);

			repo.Expect(r => r.LookupCsProjsFromDirectory(options)).Return(projects);
			slnBuilder.Expect(bld => bld.BuildSln(options)).Return(createdSlnObject);
			slnWriter.Expect(wr => wr.WriteSlnFile(createdSlnObject, ProjectsDir));

			gen.GenerateSolutionFile(options);
		}

		[TestCleanup]
		public void AfterEach()
		{
			repo.VerifyAllExpectations();
			slnWriter.VerifyAllExpectations();
			slnBuilder.VerifyAllExpectations();
		}
	}
}