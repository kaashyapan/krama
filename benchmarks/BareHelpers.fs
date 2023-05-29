namespace JsonBenchmarks

open BareNET
open Bare.Msg

module BareHelpers =
    let payloadToBare (payload: Payload) =
        let providerToBare (s: Provider) = { BProvider.Bname = s.name }

        let thumbnailToBare (x: Thumbnail option) =
            match x with
            | Some s -> { BThumbnail.Bheight = s.height; Bwidth = s.width } |> Some
            | None -> None

        let videoToBare (x: Video option) =
            match x with
            | Some s -> { BVideo.Bthumbnail = s.thumbnail |> thumbnailToBare } |> Some
            | None -> None

        let imageToBare (x: Image option) =
            match x with
            | Some s -> { BImage.Bthumbnail = s.thumbnail |> thumbnailToBare } |> Some
            | None -> None

        let valueToBare (s: SearchResult) =
            {
                BSearchResult.Bdescription = s.datePublished
                BSearchResult.BdatePublished = s.datePublished
                BSearchResult.Bimage = s.image |> imageToBare
                BSearchResult.Bvideo = s.video |> videoToBare
                BSearchResult.Bname = s.name
                BSearchResult.Bprovider = s.provider |> Seq.map providerToBare |> Seq.toArray
                BSearchResult.Burl = s.url
            }

        {
            BPayload.BreadLink = payload.readLink
            BPayload.BtotalEstimatedMatches = payload.totalEstimatedMatches
            BPayload.Bvalue = payload.value |> Seq.map valueToBare |> Seq.toArray
        }
