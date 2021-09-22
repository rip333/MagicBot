cd C:\Users\Administrator\Desktop\Repo\MagicBot
dotnet build --configuration Release
Compress-Archive -Path C:\Users\Administrator\Desktop\Repo\MagicBot\MagicBot\bin\Release\net5.0 -DestinationPath C:\Users\Administrator\Desktop\MagicBot\magicbot.zip
cd C:\Users\Administrator\Desktop\MagicBot
Expand-Archive -Path magicbot.zip