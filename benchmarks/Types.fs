namespace JsonBenchmarks

type Provider = { name: string }
type Thumbnail = { width: int; height: int }

type Image = { thumbnail: Thumbnail option }

type Video = { thumbnail: Thumbnail option }

type SearchResult =
    {
        name: string
        url: string
        description: string
        datePublished: string
        image: Image option
        video: Video option
        provider: Provider list
    }

type Payload = { readLink: string; totalEstimatedMatches: int; value: SearchResult list }
