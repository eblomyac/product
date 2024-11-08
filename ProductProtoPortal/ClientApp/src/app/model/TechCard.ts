export interface TechCard {
  id:          string;
  article:     string;
  articleLow:  string;
  identity:    string;
  identityLow: string;
  images:    Image[];
  description: null;
  postParts:   PostPart[];
}


export interface Image {
  id:              string;
  initialFileName: string;
  localPath:       string;
  url:             string;
  description:     string;
}

export interface PostPart {
  id:         string;
  techCardId: string;
  postId:     string;
  post:       string;
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
  cost:            number;
}
