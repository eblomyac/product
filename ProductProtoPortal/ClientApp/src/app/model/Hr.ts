export interface ProductTarget {
  id: number;
  postId: string;
  productWorkerId: number;

  targetName: string;

  targetCrpCenter: string;
  targetCrpPost: string;
  targetCrpPostDescription: string;
  targetCrpCenterDescription: string;
}

export interface ProductWorker {
  id: number;
  name: string;
  title: string;
  isActive:boolean;
  targets: ProductTarget[];
}

export interface ProductCalendarRecord{
  id: number;
  postId: string;
  productWorkerName: string;
  targetName: string;

  targetCrpCenter: string;
  targetCrpPost: string;
  targetCrpPostDescription: string;
  targetCrpCenterDescription: string;

  day: number;
  month: number;
  year: number;

  planningHours: number;
  description: string;

  effectiveHours: number;
  planToWorkConst: number;
}
