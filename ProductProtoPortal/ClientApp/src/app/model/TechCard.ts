export interface TechCard {
  id:          string;
  article:     string;
  articleLow:  string;
  identity:    null;
  identityLow: string;
  imageSetId:  null;
  imageSet:    ImageSet;
  description: null;
  postParts:   PostPart[];
}

export interface ImageSet {
  id:      string;
  imageId: string;
  images:  Image[];
}

export interface Image {
  id:              string;
  initialFileName: null;
  localPath:       null;
  url:             string;
  description:     string;
}

export interface PostPart {
  id:         string;
  techCardId: string;
  postId:     string;
  post:       null;
  imageSetId: null;
  imageSet:   null;
  lines:      Line[];
}

export interface Line {
  id:              string;
  techCardPostId:  string;
  isCustom:        boolean;
  processionOrder: number;
  operation:       string;
  device:          string;
  equipment:       string;
  equipmentInfo:   string;
  imageSetId:      null;
  cost:            number;
}
