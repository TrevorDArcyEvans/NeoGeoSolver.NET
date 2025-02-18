namespace NeoGeoSolver.NET.Tests.Constraints;

[TestFixture]
public sealed class PointsDistanceHorizontal_Tests
{
  [TestCase(10)]
  [TestCase(0)]
  public void PointsDistance_works(double dist)
  {
    var p0 = new Point(0, 0, 0);
    var p1 = new Point(5, 0, 0);
    var constr = new PointsDistanceHorizontal(p0, p1);
    constr.SetValue(dist);
    var eqnSys = new EquationSystem();
    eqnSys.AddEquations(constr.Equations);
    eqnSys.AddParameter(p1.X);

    var result = eqnSys.Solve();

    using (new AssertionScope())
    {
      result.Should().Be(EquationSystem.SolveResult.Okay);
      p1.X.Value.Should().BeApproximately(dist, 1e-6);
    }
  }
}