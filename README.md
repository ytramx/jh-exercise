# jh-exercise

The intention of this application is to poll subreddits for recent posts and perform a basic analysis of posts with the highest upvotes, and authors with the most number of posts.

# Configuration
appsettings.json is where the configuration values are stored.  See each section below:

**RedditConfiguration**
* RedditAppId: A valid app id from https://www.reddit.com/prefs/apps
* AccessToken: A valid access token for the Reddit API.  I used https://www.reddit.com/api/v1/access_token with a grant_type of password.

**PollingConfiguration**
* Interval: The number of seconds between attempts to retrieve posts from the subreddit.
* RangeSize: The number of posts to retrieve per execution of the background task.

**PersistenceConfiguration**
* SaveLocation: This is where completed samples will be saved for retrieval later.