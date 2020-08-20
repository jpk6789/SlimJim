using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimJim.Model;
using System.Collections.Generic;

namespace SlimJim.Test.Model.SlnBuilder
{
	[TestClass]
	public class SlnBuilderTests : SlnBuilderTestFixture
	{
		[TestMethod]
		public void SlnNameIsEqualToRootProjectName()
		{
			GeneratePartialGraphSolution(new[] { targetProjectName });

			Assert.AreEqual(targetProjectName, solution.Name);
			//Assert.That(solution.Name, Is.EqualTo(targetProjectName));
		}

		[TestMethod]
		public void SlnVersionEqualToVersionFromOptions()
		{
			options.VisualStudioVersion = VisualStudioVersion.VS2008;
			GeneratePartialGraphSolution(new string[0]);

			Assert.AreEqual(VisualStudioVersion.VS2008, solution.Version);
			//Assert.That(solution.Version, Is.EqualTo(VisualStudioVersion.VS2008));
		}

		[TestMethod]
		public void EmptySln()
		{
			GeneratePartialGraphSolution(new[] { targetProjectName });

			CollectionAssert.AreEqual(new List<CsProj>(), solution.Projects);
			//Assert.That(solution.Projects, Is.Empty);
		}

		[TestMethod]
		public void ProjectListDoesNotContainTargetProject()
		{
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.Unrelated1);

			CollectionAssert.AreEqual(new List<CsProj>(), solution.Projects);
			//Assert.That(solution.Projects, Is.Empty);
		}

		[TestMethod]
		public void ReferencesAssemblyNotInProjectsList()
		{
			projects.MyProject.ReferencesAssemblies(projects.TheirProject1);
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject);

			CollectionAssert.AreEqual(new[] { projects.MyProject }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] {projects.MyProject}));
		}

		[TestMethod]
		public void SingleProjectSln()
		{
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject);

			CollectionAssert.AreEqual(new[] { projects.MyProject }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] {projects.MyProject}));
		}

		[TestMethod]
		public void SingleEfferentAssemblyReference()
		{
			projects.MyProject.ReferencesAssemblies(projects.TheirProject1);
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.TheirProject1);

			CollectionAssert.AreEqual(new[] { projects.MyProject }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] {projects.MyProject}));
		}

		[TestMethod]
		public void UnrelatedProjectListProducesSingleProjectGraph()
		{
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.Unrelated1, projects.Unrelated2);

			CollectionAssert.AreEqual(new[] { projects.MyProject }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] {projects.MyProject}));
		}

		[TestMethod]
		public void SingleEfferentAssemblyReferenceAndUnRelatedProjectInList()
		{
			projects.MyProject.ReferencesAssemblies(projects.TheirProject1);
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.TheirProject1, projects.Unrelated1);

			CollectionAssert.AreEqual(new[] { projects.MyProject }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] { projects.MyProject }));
		}

		[TestMethod]
		public void SingleEfferentAssemblyReferenceToSubtree()
		{
			projects.MyProject.ReferencesAssemblies(projects.TheirProject1);
			projects.TheirProject1.ReferencesAssemblies(projects.TheirProject2, projects.TheirProject3);
			GeneratePartialGraphSolution(new[] { targetProjectName }, new[]
				{
					projects.MyProject, projects.TheirProject1, projects.TheirProject2, projects.TheirProject3
				});

			CollectionAssert.AreEqual(new[] { projects.MyProject }, solution.Projects);

			//Assert.That(solution.Projects, Is.EqualTo(new[]
			//	{
			//		projects.MyProject
			//	}));
		}

		[TestMethod]
		public void SingleEfferentProjectReference()
		{
			projects.MyProject.ReferencesProjects(projects.TheirProject1);
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.TheirProject1);

			CollectionAssert.AreEqual(new[] { projects.MyProject, projects.TheirProject1 }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] {projects.MyProject, projects.TheirProject1}));
		}

		[TestMethod]
		public void SingleEfferentProjectReferenceToSubtree()
		{
			projects.MyProject.ReferencesProjects(projects.TheirProject1);
			projects.TheirProject1.ReferencesProjects(projects.TheirProject2, projects.TheirProject3);
			GeneratePartialGraphSolution(new[] { targetProjectName }, new[]
				{
					projects.MyProject, projects.TheirProject1, projects.TheirProject2, projects.TheirProject3
				});

			CollectionAssert.AreEqual(new[] 
				{
					projects.MyProject, projects.TheirProject1, projects.TheirProject2, projects.TheirProject3
				}, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[]
			//	{
			//		projects.MyProject, projects.TheirProject1, projects.TheirProject2, projects.TheirProject3
			//	}));
		}

		[TestMethod]
		public void ReferencesProjectNotInProjectsList()
		{
			projects.MyProject.ReferencesProjects(projects.TheirProject1);
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject);

			CollectionAssert.AreEqual(new[] { projects.MyProject }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] {projects.MyProject}));
		}

		[TestMethod]
		public void MultipleAfferentAssemblyReferences()
		{
			projects.OurProject1.ReferencesAssemblies(projects.MyProject);
			projects.OurProject2.ReferencesAssemblies(projects.MyProject);
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.OurProject1, projects.OurProject2);

			CollectionAssert.AreEqual(new[] { projects.MyProject, projects.OurProject1, projects.OurProject2 }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] {projects.MyProject, projects.OurProject1, projects.OurProject2}));
		}

		[TestMethod]
		public void AfferentAssemblyReferencesSelectsTargetFramework()
		{
			projects.MyProject.ReferencesAssemblies(projects.MyMultiFrameworkProject35);
			options.IncludeEfferentAssemblyReferences = true;
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.MyMultiFrameworkProject35, projects.MyMultiFrameworkProject40);

			CollectionAssert.AreEqual(new[] { projects.MyProject, projects.MyMultiFrameworkProject35 }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] { projects.MyProject, projects.MyMultiFrameworkProject35 }));
		}

		[TestMethod]
		public void AfferentAssemblyReferencesSelectsNearestTargetFramework()
		{
			projects.MyProject.TargetFrameworkVersion = "v4.5";
			projects.MyProject.ReferencesAssemblies(projects.MyMultiFrameworkProject40);
			options.IncludeEfferentAssemblyReferences = true;
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.MyMultiFrameworkProject35, projects.MyMultiFrameworkProject40);

			CollectionAssert.AreEqual(new[] { projects.MyProject, projects.MyMultiFrameworkProject40 }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] { projects.MyProject, projects.MyMultiFrameworkProject40 }));
		}

		[TestMethod]
		public void MultipleAfferentProjectReferences()
		{
			projects.OurProject1.ReferencesProjects(projects.MyProject);
			projects.OurProject2.ReferencesProjects(projects.MyProject);
			GeneratePartialGraphSolution(new[] { targetProjectName }, projects.MyProject, projects.OurProject1, projects.OurProject2);

			CollectionAssert.AreEqual(new[] { projects.MyProject, projects.OurProject1, projects.OurProject2 }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[] { projects.MyProject, projects.OurProject1, projects.OurProject2 }));
		}

		[TestMethod]
		public void AfferentAssemblyReferenceReferencingOtherProjects()
		{
			projects.OurProject1.ReferencesAssemblies(projects.MyProject);
			projects.OurProject1.ReferencesAssemblies(projects.Unrelated1);
			projects.OurProject1.ReferencesAssemblies(projects.Unrelated2);
			GeneratePartialGraphSolution(new[] { targetProjectName }, new[]
				{
					projects.MyProject, projects.OurProject1, projects.Unrelated1, projects.Unrelated2
				});

			CollectionAssert.AreEqual(new[] { projects.MyProject, projects.OurProject1 }, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[]
			//	{
			//		projects.MyProject, projects.OurProject1
			//	}));
		}

		[TestMethod]
		public void MixedAfferentAndEfferentReferences()
		{
			projects.OurProject1.ReferencesAssemblies(projects.MyProject);
			projects.OurProject1.ReferencesProjects(projects.TheirProject1);
			projects.OurProject2.ReferencesProjects(projects.MyProject);
			projects.OurProject2.ReferencesAssemblies(projects.TheirProject2);
			projects.MyProject.ReferencesAssemblies(projects.TheirProject1, projects.TheirProject2);
			projects.MyProject.ReferencesProjects(projects.TheirProject3);
			projects.TheirProject3.ReferencesAssemblies(projects.Unrelated1);
			projects.TheirProject3.ReferencesProjects(projects.Unrelated2);
			GeneratePartialGraphSolution(new[] {targetProjectName}, new[]
				{
					projects.MyProject, projects.TheirProject1, projects.TheirProject2, projects.TheirProject3,
					projects.OurProject1, projects.OurProject2, projects.Unrelated1, projects.Unrelated2
				});

			CollectionAssert.AreEqual(new[]
				{
					projects.MyProject, projects.TheirProject3, projects.Unrelated2,
					projects.OurProject1, projects.TheirProject1, projects.OurProject2
				}, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[]
			//	{
			//		projects.MyProject, projects.TheirProject3, projects.Unrelated2,
			//		projects.OurProject1, projects.TheirProject1, projects.OurProject2
			//	}));
		}

		[TestMethod]
		public void MultipleTargetProjects()
		{
			projects.OurProject2.ReferencesAssemblies(projects.MyProject, projects.OurProject1);
			projects.MyProject.ReferencesProjects(projects.TheirProject1);
			projects.OurProject1.ReferencesProjects(projects.TheirProject2);
			GeneratePartialGraphSolution(new[] { projects.MyProject.AssemblyName, projects.OurProject1.AssemblyName }, new[]
				{
					projects.MyProject, projects.OurProject1, projects.OurProject2, 
					projects.TheirProject1, projects.TheirProject2
				});

			CollectionAssert.AreEqual(new[]
				{
					projects.MyProject, projects.TheirProject1, projects.OurProject2,
					projects.OurProject1, projects.TheirProject2
				}, solution.Projects);
			//Assert.That(solution.Projects, Is.EqualTo(new[]
			//	{
			//		projects.MyProject, projects.TheirProject1, projects.OurProject2,
			//		projects.OurProject1, projects.TheirProject2
			//	}));
		}
	}
}