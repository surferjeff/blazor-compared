$(cd BlazorApp && dotnet run -c Release) &
$(cd RazorApp && dotnet run -c Release) &
$(cd GoApp && go run . > /dev/null) &
