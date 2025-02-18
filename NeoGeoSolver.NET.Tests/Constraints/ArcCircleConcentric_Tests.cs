namespace NeoGeoSolver.NET.Tests.Constraints;

[TestFixture]
public sealed class ArcCircleConcentric_Tests
{
  [Test]
  public void Concentric_works()
  {
    var centre0 = new Point(0, 0, 0);
    var radius0 = new Param("radius0", 10);
    var startAngle = new Param("startAngle", 0);
    var endAngle = new Param("endAngle", Math.PI);
    var arc0 = new Arc(centre0, radius0, startAngle, endAngle);
    var centre1 = new Point(10, 0, 0);
    var radius1 = new Param("radius1", 10);
    var circle1 = new Circle(centre1, radius1);
    var constr = new ArcCircleConcentric(arc0, circle1);
    var eqnSys = new EquationSystem();
    eqnSys.AddEquations(constr.Equations);
    eqnSys.AddParameter(centre1.X);

    var result = eqnSys.Solve();

    using (new AssertionScope())
    {
      result.Should().Be(EquationSystem.SolveResult.Okay);
      centre1.X.Value.Should().BeApproximately(0, 1e-4);
    }
  }
}
