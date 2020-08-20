using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlimJim.Test.Model.SlnBuilder
{
	[TestClass]
	public class IncludeAllProjectsInSln : SlnBuilderTestFixture
	{
		[TestMethod]
		public void WithNoTargetsInOptionsAllProjectsAreIncluded()
		{
			GeneratePartialGraphSolution(new string[0], projects.MyProject, projects.OurProject1, 
				projects.TheirProject1, projects.Unrelated1);

			CollectionAssert.AreEqual(new[] {projects.MyProject, projects.OurProject1,
				projects.TheirProject1, projects.Unrelated1}, solution.Projects);
		}
	}
}