export interface Transfer {
  id: number
  paperId: string
  postFromId: string
  postToId: string
  created: Date
  createdStamp: Date
  closed: Date
  closedStamp: Date
  createdBy: string
  closedBy: string
  lines: TransferLine[]
  totalItemsCount:number;
  totalItemsTransfered:number;
  orders:string;
}

export interface TransferLine {
  id: number
  article: string
  orderNumber: number
  count: number
  productionLine: string
  isTransfered: boolean
  transferedCount: number
  remark: string
  sourceWorkId: number
  sourceWorkCost: number
  targetWorkId: number
}
