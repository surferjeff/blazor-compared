$(cd BlazorApp && dotnet run -c Release > /dev/null) &
$(cd RazorApp && dotnet run -c Release > /dev/null) &
$(cd GoApp && go run . > /dev/null) &
