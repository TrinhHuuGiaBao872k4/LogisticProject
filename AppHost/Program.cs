var builder = DistributedApplication.CreateBuilder(args);

// var BlazorApp = builder.AddProject<Projects.LogisticBlazorWebApp>("logistic-blazor-web-app");
var LogisticApi = builder.AddProject<Projects.LogisticService>("logistic-api");
var PaymentApi = builder.AddProject<Projects.PaymentService>("payment-api");
var GateWay = builder.AddProject<Projects.GateWayService>("gate-way-app");

builder.AddProject<Projects.LogisticBlazorWebApp>("logistic-blazor-web-app")
.WithReference(LogisticApi)
.WithReference(PaymentApi)
.WithReference(GateWay);
builder.Build().Run();
