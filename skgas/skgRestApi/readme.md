버전확인
dotnet ef --version


.NET 8에 맞는 EF Tool로 재설치


dotnet tool uninstall --global dotnet-ef


dotnet tool install --global dotnet-ef --version 8.*

확인

dotnet ef --version