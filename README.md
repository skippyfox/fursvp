# fursvp
A tool for communities to create quick and easy public sign-up sheets with no account required.

## What does it do?
Fursvp gives your community a central hub for organizers and members to plan events in the real world. At the most basic level, it allows organizers to quickly create a page for an event in just a few steps, and attendees do not need to create an account to sign up or see who else is coming.

## Who can use this?
This tool is intended to replace the closed-source Fursvp.com that is currently in use by furry fandom communities. Other communities or subcommunities are welcome to build and deploy for their own use. The design is intended to be containerized and able to run with a nosql database on the "free" tier of major cloud hosting platforms like Google Cloud Platform and AWS.

## What about other apps like Meetup.com, Facebook events and Google Forms?
Although these tools are similar, and many organizers may find them suitable as they are, each of them has some shortcoming that Fursvp aims to make up for.

Organizers may find that members of their community span across many different social media platforms such as Meetup.com, Facebook or community forums, and there is no single service that all members use. There are reasons for potential attendees to be reluctant to create a new user account on a platform they don't intend to use except to review and sign up for an event. Fursvp does not aim to be "yet another app" for all parties involved to keep track of and maintain accounts for. Hence, one of its goals is accessibility and ease for all members of your community.

Google Forms is an almost perfect RSVP tool. It lacks two notable features: A public sign-up sheet (or RSVP list) and a persistent details page. The former might not be a must-have for your community, but the latter is. The entire form and all details become hidden as soon as the form is closed to a member (such as after submitting a form that is limited to one per Google account, or after an arbitrary deadline is passed). If you do prefer to use Google Forms, I strongly recommend using it ONLY for signups, and keep important event details published somewhere else that is accessible and won't disappear!

## What features are planned?
- Browser and mobile browser web UIs
- Standalone backend API that can be used by multiple or custom UIs
- Accessible and easy to use: Setting up an event is easy; signing up for an event is trivial. No account creation required.
- Containerized codebase - able to run with a nosql database on the "free" tier of Google Cloud Platform or AWS
- Customizable event pages (details, activities, rules, contact info); no knowledge of CSS, HTML, formatting or graphic design necessary
- Email-based identification for control panel access

### Nice-To-Haves:
- Social media integration (share your event; designate a photo gallery)
- Geographical features (find events nearby, view map of upcoming events)
- Calendar views and calendar app support
- Limited attendance and Wait Lists
- Designated co-organizers
- Revisit past/archived events
- Email notifications (event updates, new events, reminders for upcoming deadlines)
- SMS notifications
- Buildable/customizable sign-up forms
- Custom form options with limited availability
- Moderated sign-ups (hidden until approved)

### Maybe someday:
- Third-party payment integration for ticket purchases

## How do I get started? How can I help?
If you're reading this in mid-2020, this source is nearly functional! We could probably use some help! If you'd like to be a contributor, well, THANKS! Having outside help is the biggest reasons this project is open source. Check the Issues section to see what's still needed.

We don't have an established set of Contributing Guidelines yet, but we do have a [Code of Conduct](CODE_OF_CONDUCT.md). [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg)](CODE_OF_CONDUCT.md)

Until this code is functional, you can check out the old closed-source fursvp.com.

### What you'll need
At a minimum, you will need Docker (or Docker Desktop) to run this codebase. 

Having Visual Studio is recommended, but the project can be built entirely within containers. Visual Studio provides you the option of running it outside of a container. You can work against a Google Firestore database by copying your service account key to %appdata%\Microsoft\UserSecrets\fursvp\firestore.json.

### Building With Docker

```bash
cd src
docker build -t fursvp .
docker run -p 80:80 --env-file=fursvp.api/settings.env fursvp:latest
```

To persist data in Firebase, some additional configuration is needed.

1. Edit src/fursvp.api/settings.env and set GOOGLE_APPLICATION_CREDENTIALS=/tmp/keys/gcp.json
2. Export a variable with the path of your GCP service account json
```bash
export GOOGLE_APPLICATION_CREDENTIALS=/local/path/to/gcp.json
```
3. Mount the path as shown below
```bash
docker run -p 80:80 --env-file fursvp.api/settings.env -v$GOOGLE_APPLICATION_CREDENTIALS:/tmp/keys/gcp.json:ro fursvp:latest
```

## License
This project is licensed under the MIT License - see the LICENSE.md file for details

## Acknowledgments
The original closed-source version of Fursvp was a collaboration between LupineFox, Protocollie and skippyfox back in 2008 called "Surple." It was started in classic ASP before being rewritten in .NET Web Forms in 2010, though the same blue UI and SQL Server backend remain in 2020. While the Open Source Fursvp is a complete rewrite leaving all of that behind, everything is owed to LupineFox and Protocollie for the initial commitment that brought the project to life, as well as to Kitt3ns and other early adopters for the gratitude and support that kept it going!
