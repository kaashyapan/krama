//////////////////////////////////////////////////
// Generated code by BareNET - 11/26/2022 9:29 PM //
//////////////////////////////////////////////////
namespace Bare.Msg

type BProvider = { Bname: string }

type BThumbnail = { Bwidth: int; Bheight: int }

type BImage = { Bthumbnail: BThumbnail option }

type BVideo = { Bthumbnail: BThumbnail option }

type BSearchResult =
    {
        Bname: string
        Burl: string
        Bdescription: string
        BdatePublished: string
        Bimage: BImage option
        Bvideo: BVideo option
        Bprovider: BProvider array
    }

type BPayload = { BreadLink: string; BtotalEstimatedMatches: int; Bvalue: BSearchResult array }

module Encoding =
    let ofBProvider (value: BProvider) : BareNET.Encoding_Result =
        (BareNET.Bare.success (BareNET.Bare.encode_string)) value.Bname

    let decode_BProvider: BareNET.Decoder<BProvider> =
        BareNET.Bare.decode_complex (fun (bname: string) -> { BProvider.Bname = bname })
        |> BareNET.Bare.apply (BareNET.Bare.decode_string)

    let toBProvider (data: byte array) : Result<BProvider, string> = decode_BProvider data |> Result.map fst

    let ofBThumbnail (value: BThumbnail) : BareNET.Encoding_Result =
        (BareNET.Bare.success (BareNET.Bare.encode_i32)) value.Bwidth
        |> BareNET.Bare.andThen (BareNET.Bare.success (BareNET.Bare.encode_i32)) value.Bheight

    let decode_BThumbnail: BareNET.Decoder<BThumbnail> =
        BareNET.Bare.decode_complex (fun (bwidth: int) (bheight: int) ->
            { BThumbnail.Bwidth = bwidth; Bheight = bheight }
        )
        |> BareNET.Bare.apply (BareNET.Bare.decode_i32)
        |> BareNET.Bare.apply (BareNET.Bare.decode_i32)

    let toBThumbnail (data: byte array) : Result<BThumbnail, string> = decode_BThumbnail data |> Result.map fst

    let ofBImage (value: BImage) : BareNET.Encoding_Result =
        (BareNET.Bare.encode_optional (ofBThumbnail)) value.Bthumbnail

    let decode_BImage: BareNET.Decoder<BImage> =
        BareNET.Bare.decode_complex (fun (bthumbnail: BThumbnail option) -> { BImage.Bthumbnail = bthumbnail })
        |> BareNET.Bare.apply (BareNET.Bare.decode_optional (decode_BThumbnail))

    let toBImage (data: byte array) : Result<BImage, string> = decode_BImage data |> Result.map fst

    let ofBVideo (value: BVideo) : BareNET.Encoding_Result =
        (BareNET.Bare.encode_optional (ofBThumbnail)) value.Bthumbnail

    let decode_BVideo: BareNET.Decoder<BVideo> =
        BareNET.Bare.decode_complex (fun (bthumbnail: BThumbnail option) -> { BVideo.Bthumbnail = bthumbnail })
        |> BareNET.Bare.apply (BareNET.Bare.decode_optional (decode_BThumbnail))

    let toBVideo (data: byte array) : Result<BVideo, string> = decode_BVideo data |> Result.map fst

    let ofBSearchResult (value: BSearchResult) : BareNET.Encoding_Result =
        (BareNET.Bare.success (BareNET.Bare.encode_string)) value.Bname
        |> BareNET.Bare.andThen (BareNET.Bare.success (BareNET.Bare.encode_string)) value.Burl
        |> BareNET.Bare.andThen (BareNET.Bare.success (BareNET.Bare.encode_string)) value.Bdescription
        |> BareNET.Bare.andThen (BareNET.Bare.success (BareNET.Bare.encode_string)) value.BdatePublished
        |> BareNET.Bare.andThen (BareNET.Bare.encode_optional (ofBImage)) value.Bimage
        |> BareNET.Bare.andThen (BareNET.Bare.encode_optional (ofBVideo)) value.Bvideo
        |> BareNET.Bare.andThen (BareNET.Bare.encode_list (ofBProvider)) value.Bprovider

    let decode_BSearchResult: BareNET.Decoder<BSearchResult> =
        BareNET.Bare.decode_complex (fun
                                         (bname: string)
                                         (burl: string)
                                         (bdescription: string)
                                         (bdatePublished: string)
                                         (bimage: BImage option)
                                         (bvideo: BVideo option)
                                         (bprovider: BProvider array) ->
            {
                BSearchResult.Bname = bname
                Burl = burl
                Bdescription = bdescription
                BdatePublished = bdatePublished
                Bimage = bimage
                Bvideo = bvideo
                Bprovider = bprovider
            }
        )
        |> BareNET.Bare.apply (BareNET.Bare.decode_string)
        |> BareNET.Bare.apply (BareNET.Bare.decode_string)
        |> BareNET.Bare.apply (BareNET.Bare.decode_string)
        |> BareNET.Bare.apply (BareNET.Bare.decode_string)
        |> BareNET.Bare.apply (BareNET.Bare.decode_optional (decode_BImage))
        |> BareNET.Bare.apply (BareNET.Bare.decode_optional (decode_BVideo))
        |> BareNET.Bare.apply (
            BareNET.Bare.decode_complex Array.ofSeq
            |> BareNET.Bare.apply (BareNET.Bare.decode_list (decode_BProvider))
        )

    let toBSearchResult (data: byte array) : Result<BSearchResult, string> = decode_BSearchResult data |> Result.map fst

    let ofBPayload (value: BPayload) : BareNET.Encoding_Result =
        (BareNET.Bare.success (BareNET.Bare.encode_string)) value.BreadLink
        |> BareNET.Bare.andThen (BareNET.Bare.success (BareNET.Bare.encode_i32)) value.BtotalEstimatedMatches
        |> BareNET.Bare.andThen (BareNET.Bare.encode_list (ofBSearchResult)) value.Bvalue

    let decode_BPayload: BareNET.Decoder<BPayload> =
        BareNET.Bare.decode_complex (fun (breadLink: string) (btotalEstimatedMatches: int) (bvalue: BSearchResult array) ->
            { BPayload.BreadLink = breadLink; BtotalEstimatedMatches = btotalEstimatedMatches; Bvalue = bvalue }
        )
        |> BareNET.Bare.apply (BareNET.Bare.decode_string)
        |> BareNET.Bare.apply (BareNET.Bare.decode_i32)
        |> BareNET.Bare.apply (
            BareNET.Bare.decode_complex Array.ofSeq
            |> BareNET.Bare.apply (BareNET.Bare.decode_list (decode_BSearchResult))
        )

    let toBPayload (data: byte array) : Result<BPayload, string> = decode_BPayload data |> Result.map fst
