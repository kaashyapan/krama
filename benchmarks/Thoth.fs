namespace JsonBenchmarks

open Thoth
open Thoth.Json.Net

module ThothEncoders =
    let toJsonThumbnail (o: Thumbnail option) =
        match o with
        | Some j -> Encode.object [ ("height", Encode.int j.height); ("width", Encode.int j.width) ]
        | None -> Encode.nil

    let toJsonImage (o: Image option) =
        match o with
        | Some j -> Encode.object [ ("thumbnail", toJsonThumbnail j.thumbnail) ]
        | None -> Encode.nil

    let toJsonVideo (o: Video option) =
        match o with
        | Some j -> Encode.object [ ("thumbnail", toJsonThumbnail j.thumbnail) ]
        | None -> Encode.nil

    let toJsonProvider (o: Provider list) =
        o
        |> List.map (fun x -> Encode.object [ "name", Encode.string x.name ])
        |> Encode.list

    let toJsonSearchResult (o: SearchResult list) =
        o
        |> List.map (fun s ->
            [
                ("name", Encode.string s.name)
                ("image", toJsonImage s.image)
                ("url", Encode.string s.url)
                ("description", Encode.string s.description)
                ("datePublished", Encode.string s.datePublished)
                ("video", toJsonVideo s.video)
                ("provider", toJsonProvider s.provider)
            ]
            |> Encode.object
        )
        |> Encode.list

    let toJsonPayload (o: Payload) =
        let results = toJsonSearchResult o.value

        [
            ("readLink", Encode.string o.readLink)
            ("totalEstimatedMatches", Encode.int o.totalEstimatedMatches)
            ("value", results)
        ]
        |> Encode.object

module ThothDecoders =
    let thumbnailDecoder =
        Decode.object (fun get ->
            {
                Thumbnail.height = get.Required.Field "height" Decode.int
                width = get.Required.Field "width" Decode.int
            }
        )

    let imageDecoder = Decode.object (fun get -> { Image.thumbnail = get.Optional.Field "thumbnail" thumbnailDecoder })

    let videoDecoder = Decode.object (fun get -> { Video.thumbnail = get.Optional.Field "thumbnail" thumbnailDecoder })

    let providerDecoder = Decode.object (fun get -> { Provider.name = get.Required.Field "name" Decode.string })

    let searchResultDecoder =
        Decode.object (fun get ->
            {
                SearchResult.name = get.Required.Field "name" Decode.string
                url = get.Required.Field "url" Decode.string
                description = get.Required.Field "description" Decode.string
                datePublished = get.Required.Field "datePublished" Decode.string
                image = get.Optional.Field "image" imageDecoder
                video = get.Optional.Field "video" videoDecoder
                provider = get.Required.Field "provider" (Decode.list providerDecoder)
            }
        )

    let payloadDecoder: (string -> JsonValue -> Result<Payload, DecoderError>) =
        Decode.object (fun get ->
            {
                Payload.readLink = get.Required.Field "readLink" Decode.string
                totalEstimatedMatches = get.Required.Field "totalEstimatedMatches" Decode.int
                value = get.Required.Field "value" (Decode.list searchResultDecoder)
            }
        )
