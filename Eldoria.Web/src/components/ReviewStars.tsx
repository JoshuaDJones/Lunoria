import { faStarHalf } from "@fortawesome/free-solid-svg-icons";
import { faStar } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { ReactElement, useEffect, useState } from "react";
import EasyText from "./EasyText";

enum StarType {
  none,
  half,
  full,
}

interface ReviewStarsProps {
  reviewAverage: number;
  reviewCount: number;
}

const ReviewStars = ({ reviewAverage, reviewCount }: ReviewStarsProps) => {
  const [stars, setStars] = useState<StarType[]>([]);

  useEffect(() => {
    let starsArr = [...stars];

    for (let i = 0; i < 5; i++) {
      let remainder = reviewAverage - i;

      if (remainder > 0.75) starsArr[i] = StarType.full;

      if (remainder < 0.35) starsArr[i] = StarType.none;

      if (remainder > 0.35 && remainder < 0.75) starsArr[i] = StarType.half;
    }

    setStars(starsArr);
  }, [reviewAverage]);

  return (
    <div className="flex flex-row">
      {stars.map((i, index) => (
        <Star key={index} starType={i} />
      ))}
      <EasyText className="mr-2 underline text-xl cursor-pointer">
        {reviewCount}
      </EasyText>
    </div>
  );
};

interface StarProps {
  starType: StarType;
}

const Star = ({ starType }: StarProps) => {
  let stateStar: ReactElement | undefined;

  switch (starType) {
    case StarType.none:
      stateStar = (
        <FontAwesomeIcon
          icon={faStar}
          size="lg"
          className="mr-1 absolute z-10 text-gray-200 dark:text-gray-700"
        />
      );
      break;
    case StarType.half:
      stateStar = (
        <>
          <FontAwesomeIcon
            icon={faStarHalf}
            size="lg"
            className="mr-1 absolute z-10 text-yellow-500 dark:text-yellow-500"
          />
          <FontAwesomeIcon
            icon={faStar}
            size="lg"
            className="mr-1 absolute text-gray-200 dark:text-gray-700"
          />
        </>
      );
      break;
    case StarType.full:
      stateStar = (
        <FontAwesomeIcon
          icon={faStar}
          size="lg"
          className="mr-1 absolute z-10 text-yellow-500 dark:text-yellow-500"
        />
      );
      break;
    default:
      return null;
  }

  return (
    <div
      style={{ height: 25, width: 25 }}
      className="flex justify-center items-center relative"
    >
      {stateStar}
    </div>
  );
};

export default ReviewStars;
