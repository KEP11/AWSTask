FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY AwsTask .
#COPY ["AwsTask/AwsTask.csproj", ""]
RUN dotnet restore "AwsTask.csproj"

RUN dotnet build "AwsTask.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "AwsTask.csproj" -c Debug -o /app/publish

#docker run -d -p 8080:80 -e ASPNETCORE_ENVIRONMENT=development -v "%UserProfile%\.aws\credentials":/root/.aws/credentials:ro --name myapp awstask
#docker run -d -p 8080:80 -e AWS_ACCESS_KEY_ID=AKIAQTS5UI6NG7IWXXH6 -e AWS_SECRET_ACCESS_KEY=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 --name myapp awstask

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AwsTask.dll"]