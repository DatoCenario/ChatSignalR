import { Button, Input, Row, UploadFile } from "antd";
import styled from "styled-components";
import "react-image-crop/dist/ReactCrop.css";
import Icon, { CloseOutlined } from "@ant-design/icons";

export interface ImageListImage {
  url: string;
}

interface ImageListProps {
  images: ImageListImage[];
  onImageClick?: (url: ImageListImage) => void;
  onClearClick?: () => void;
}

const ImagesWrapper = styled.div`
  overflow-x: scroll;
  display: flex;
  flex-direction: row;
  border-box: solid 1px white;
`;

export const ImageList: React.FC<ImageListProps> = ({
  images,
  onImageClick,
  onClearClick,
}) => {
  return (
    <ImagesWrapper>
      <Button
        onClick={() => {
          onClearClick && onClearClick();
        }}
        style={{
          backgroundColor: "red",
        }}
        icon={<CloseOutlined />}
        size="small"
      />
      {images.map((image, index) => (
        <img
          src={image.url}
          key={index}
          style={{ width: "32px", height: "32px", marginRight: "8px" }}
          onClick={() => {
            onImageClick && onImageClick(image);
          }}
        />
      ))}
    </ImagesWrapper>
  );
};
