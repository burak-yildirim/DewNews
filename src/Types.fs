module Types

type HNItemType = HNJob | HNStory | HNComment | HNPoll | HNPollopt
/// Stories, comments, jobs, Ask HNs and even polls are just items
type HNItem =
    { Id: bigint
      Deleted: bool Option
      Type: HNItemType
      By: string
      Time: bigint
      Text: string Option
      Dead: bool Option
      Parent: bigint Option
      Poll: bigint Option
      Kids: bigint list Option
      Url: string Option
      Score: bigint Option
      Title: string Option
      Parts: bigint list Option
      Descendants: bigint Option }

let getItemTitleText (item: HNItem) : string =
    match item.Title with
    | Some title -> title
    | None -> "No-Title"

let getItemScoreText (item: HNItem) : string =
    match item.Score with
    | Some score -> sprintf "%O" score
    | None -> "0"

let getItemUrl (item: HNItem) : string =
    match item.Url with
    | Some url -> url
    | None -> sprintf "https://news.ycombinator.com/item?id=%O" item.Id
