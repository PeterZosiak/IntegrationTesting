using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

var wiremockServer = WireMockServer.Start();
Console.WriteLine($"WireMock server running at {wiremockServer.Urls[0]}");

wiremockServer.Given(Request.Create().WithPath("/users").UsingGet())
    .RespondWith(Response.Create().WithStatusCode(200).WithBody("This is coming from Wiremock"));

Console.ReadKey();
wiremockServer.Stop();