cd C:\temp\apps\magicbot
dotnet build --configuration Release
Compress-Archive -Path C:\temp\apps\magicbot\MagicBot\bin\Release\net5.0 -DestinationPath C:\Users\Administrator\Desktop\MagicBot\magicbot.zip
Stop-Process -Name "MagicBot"
cd C:\Users\Administrator\Desktop\MagicBot
del C:\Users\Administrator\Desktop\MagicBot\magicbot.zip
Expand-Archive -Path magicbot.zip