module Types

type HNItemType = HNJob | HNStory | HNComment | HNPoll | HNPollopt
/// Stories, comments, jobs, Ask HNs and even polls are just items
type HNItem =
    { Id: bigint
      Deleted: bool Option
      Type: HNItemType
      By: string
      Time: bigint
      Text: string
      Dead: bool Option
      Parent: bigint Option
      Poll: bigint Option
      Kids: bigint list Option
      Url: string Option
      Score: bigint Option
      Title: string Option
      Parts: bigint list Option
      Descendants: bigint Option }
