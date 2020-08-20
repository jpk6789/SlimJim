using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlimJim.Test
{
	[TestClass]
	public class HintPathConverterTests
	{
		[DataTestMethod]
		[DataRow(@"c:\projects\solutions\packages", @"c:\projects\myproj\src\MyProj\MyProj.csproj", @"..\..\..\solutions\packages")]
		public void CalculateRelativePathToSlimjimPackages(string solutionPath, string csProjPath, string expectedResult)
		{
			var actual = HintPathConverter.CalculateRelativePathToSlimjimPackages(solutionPath, csProjPath);

			StringAssert.Equals(expectedResult, actual);
		}
	}
}