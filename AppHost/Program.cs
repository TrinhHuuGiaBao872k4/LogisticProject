var builder = DistributedApplication.CreateBuilder(args);

// var BlazorApp = builder.AddProject<Projects.LogisticBlazorWebApp>("logistic-blazor-web-app");
var LogisticApi = builder.AddProject<Projects.LogisticService>("logistic-api");
var PaymentApi = builder.AddProject<Projects.PaymentService>("payment-api");

builder.AddProject<Projects.LogisticBlazorWebApp>("logistic-blazor-web-app")
.WithReference(LogisticApi)
.WithReference(PaymentApi);
builder.Build().Run();
