import {
  type ChangeEvent,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import { Button, CircularProgress, Stack } from "@mui/material";
import type { ProfileDto } from "../../../types/ProfileDto.tsx";
import { ProfileStyledTextField } from "../../ProfileStyledMUIComponents.tsx";
import { isMobilePhone } from "validator";
import { axiosRequestWithAutoReauth } from "../../../helpers/axiosRequestWithAutoReauth.tsx";

export default function ProfilePageProfileSection() {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [isEditMode, setIsEditMode] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  const lastKnownProfileData = useRef<ProfileDto | null>(null);

  useEffect(() => {
    if (!user) {
      return;
    }

    axiosRequestWithAutoReauth<ProfileDto>(
      {
        method: "GET",
        url: `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/profile`,
        headers: {
          Authorization: `Bearer ${user.accessToken}`,
        },
      },
      setUser,
    )
      .then((response) => {
        const { firstName, lastName, phoneNumber } = response.data;

        setFirstName(firstName);
        setLastName(lastName);
        setPhoneNumber(phoneNumber);

        lastKnownProfileData.current = {
          firstName,
          lastName,
          phoneNumber,
        };
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      });
  }, []);

  function handleFirstNameChange(event: ChangeEvent<HTMLInputElement>) {
    setFirstName(event.target.value);
  }

  function handleLastNameChange(event: ChangeEvent<HTMLInputElement>) {
    setLastName(event.target.value);
  }

  function handlePhoneNumberChange(event: ChangeEvent<HTMLInputElement>) {
    setPhoneNumber(event.target.value);
  }

  function handleEditClick() {
    setIsEditMode(true);
  }

  function handleSaveClick() {
    if (isEditMode) {
      const data = {
        firstName,
        lastName,
        phoneNumber,
      };

      setIsSubmitting(true);
      axiosRequestWithAutoReauth(
        {
          method: "PUT",
          url: `${import.meta.env.VITE_API_URL}/api/users/${user?.uid}/profile`,
          data: data,
          headers: {
            Authorization: `Bearer ${user?.accessToken}`,
          },
        },
        setUser,
      )
        .then(() => {})
        .catch((error) => {
          setFirstName(lastKnownProfileData.current?.firstName || "");
          setLastName(lastKnownProfileData.current?.lastName || "");
          setPhoneNumber(lastKnownProfileData.current?.phoneNumber || "");

          toastErrorMessageHandle(addToast, setUser, error);
        })
        .finally(() => {
          setIsSubmitting(false);
          setIsEditMode(false);
        });
    }
  }

  return (
    <Stack gap={4}>
      <ProfileStyledTextField
        readOnly={!isEditMode}
        label="First Name"
        placeholder={isEditMode ? undefined : "Empty"}
        value={firstName}
        onChange={handleFirstNameChange}
      />
      <ProfileStyledTextField
        readOnly={!isEditMode}
        label="Last Name"
        placeholder={isEditMode ? undefined : "Empty"}
        value={lastName}
        onChange={handleLastNameChange}
      />
      <ProfileStyledTextField
        readOnly={!isEditMode}
        label="Phone Number"
        placeholder={isEditMode ? undefined : "Empty"}
        error={!!phoneNumber && !isMobilePhone(phoneNumber)}
        helperText={
          !!phoneNumber && !isMobilePhone(phoneNumber)
            ? "Invalid phone number"
            : undefined
        }
        value={phoneNumber}
        onChange={handlePhoneNumberChange}
      />
      {isEditMode ? (
        <Button
          disabled={isSubmitting}
          variant="contained"
          color="primary"
          onClick={handleSaveClick}
        >
          {isSubmitting ? <CircularProgress /> : "Save Changes"}
        </Button>
      ) : (
        <Button variant="outlined" color="primary" onClick={handleEditClick}>
          Edit Profile
        </Button>
      )}
    </Stack>
  );
}
