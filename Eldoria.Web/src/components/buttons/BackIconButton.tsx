import { faArrowCircleLeft } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react'
import { useNavigate } from 'react-router'

const BackIconButton = () => {
    const navigate = useNavigate();

  return (
        <button
          className="hover:opacity-70"
          onClick={(e) => {
            e.stopPropagation();
            navigate(-1);
          }}
        >
          <FontAwesomeIcon size="4x" icon={faArrowCircleLeft} className="text-black" />
        </button>
  )
}

export default BackIconButton

