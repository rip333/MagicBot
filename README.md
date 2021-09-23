# MagicBot

[![telegram chat](https://img.shields.io/badge/Support_Chat-Telegram-blue.svg?style=flat-square)](https://t.me/joinchat/B35YY0QbLfd034CFnvCtCA)

## About

This is a telegram bot used to post images of Magic the Gathering cards into a chat.  It uses [Scryfall API](https://api.scryfall.com) for the data.

### CI/CD

- [GitHub Actions](https://github.com/rip333/MagicBot/blob/master/.github/workflows/workflow.yml)
- [AWS CodeDeploy](https://github.com/rip333/MagicBot/blob/master/appspec.yml)

## Syntax

* Surround the name of the card in [[double brackets]]
  * [[Professor Onyx]]
  * I like [[Professor Onyx]] more than [[Liliana of the Dark Realms]]
    * The bot will batch messages with multiple card requests into a single album post.
* You can also get a random card, with optional querying.
  * [[Random]]
  * [[Random t:goblin]]
  * [Query syntax](https://scryfall.com/docs/syntax)

## More Info
- [Telegram Bots](https://core.telegram.org/bots)
- [Scryfall API](https://scryfall.com/docs/api)
- [GitHub Actions](https://docs.github.com/en/actions)
- [AWS CodeDeploy](https://docs.aws.amazon.com/codedeploy/latest/userguide/welcome.html)
