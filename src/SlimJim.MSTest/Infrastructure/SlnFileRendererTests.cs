using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimJim.Infrastructure;
using SlimJim.Model;
using SlimJim.Test.Model;
using SlimJim.Test.SampleFiles;

namespace SlimJim.Test.Infrastructure
{
	[TestClass]
	public class SlnFileRendererTests
	{
		private Sln solution;
		private SlnFileRenderer renderer;
		private ProjectPrototypes projects;

		[TestInitialize]
		public void BeforeEach()
		{
			projects = new ProjectPrototypes();
		}

		[TestMethod]
		public void EmptySolution()
		{
			MakeSolution("BlankSolution");

			TestRender();
		}

		[TestMethod]
		public void SingleProjectSolution()
		{
			MakeSolution("SingleProject", projects.MyProject);

			TestRender();
		}

		[TestMethod]
		public void ThreeProjectSolution()
		{
			MakeSolution("ThreeProjects", projects.MyProject, projects.OurProject1, projects.OurProject2);

			TestRender();
		}

		[TestMethod]
		public void ManyProjectSolution()
		{
			MakeSolution("ManyProjects", projects.MyProject, projects.OurProject1, projects.OurProject2,
				projects.TheirProject1, projects.TheirProject2, projects.TheirProject3);

			TestRender();
		}

		[TestMethod]
		public void VisualStudio2008Solution()
		{
			MakeSolution("VS2008");
			solution.Version = VisualStudioVersion.VS2008;

			TestRender();
		}

        [TestMethod]
        public void VisualStudio2015Solution()
        {
            MakeSolution("VS2015");
            solution.Version = VisualStudioVersion.VS2015;

            TestRender();
        }

        [TestMethod]
        public void VisualStudio2017Solution()
        {
            MakeSolution("VS2017");
            solution.Version = VisualStudioVersion.VS2017;

            TestRender();
        }

        private void MakeSolution(string name, params CsProj[] csProjs)
		{
			solution = new Sln(name, "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
            solution.Version = VisualStudioVersion.VS2010;
			solution.AddProjects(csProjs);
		}

		private void TestRender()
		{
            renderer = new SlnFileRenderer(solution);

            string actualContents = renderer.Render().Replace("\r\n", "\n").Replace("\n\n", "\n");
			string expectedContents = SampleFileHelper.GetSlnFileContents(solution.Name).Replace("\r\n", "\n").Replace("\n\n", "\n");

			Assert.AreEqual(expectedContents, actualContents);
		}
	}
}