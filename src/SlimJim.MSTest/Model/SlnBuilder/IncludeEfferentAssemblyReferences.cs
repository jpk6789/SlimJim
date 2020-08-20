using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlimJim.Test.Model.SlnBuilder
{
	[TestClass]
	public class IncludeEfferentAssemblyReferences : SlnBuilderTestFixture
	{
		[TestMethod]
		public void EfferentAssemblyReferencesAreIncludedForAllProjectsInSln()
		{
			options.IncludeEfferentAssemblyReferences = true;
			projects.MyProject.ReferencesAssemblies(projects.TheirProject1, projects.TheirProject2);
			projects.TheirProject2.ReferencesProjects(projects.TheirProject3);
			projects.OurProject1.ReferencesAssemblies(projects.MyProject, projects.OurProject2);
			GeneratePartialGraphSolution(new[] {targetProjectName},
										 projects.MyProject,
										 projects.TheirProject1,
										 projects.TheirProject2,
										 projects.TheirProject3,
										 projects.OurProject1,
										 projects.OurProject2);

			CollectionAssert.AreEqual(new[]{
												projects.MyProject,
												projects.TheirProject1,
												projects.TheirProject2,
												projects.TheirProject3,
												projects.OurProject1,
												projects.OurProject2
											}, solution.Projects);
		}
	}
}