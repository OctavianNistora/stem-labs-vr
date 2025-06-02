import {
  Box,
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";
import axios from "axios";
import { useContext, useState, type ChangeEvent } from "react";
import { isMobilePhone } from "validator";
import { useNavigate } from "react-router";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import isEmail from "validator/lib/isEmail";

type SelectOption = {
  value: string;
  label: string;
};

const Options: SelectOption[] = [
  { value: "student", label: "Student" },
  { value: "professor", label: "Professor" },
];

export default function AddUserPage() {
  const [email, setEmail] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [role, setRole] = useState(Options[0].value);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigate = useNavigate();
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  function handleEmailChange(event: ChangeEvent<HTMLInputElement>) {
    setEmail(event.target.value);
  }

  function handleFirstNameChange(event: ChangeEvent<HTMLInputElement>) {
    setFirstName(event.target.value);
  }

  function handleLastNameChange(event: ChangeEvent<HTMLInputElement>) {
    setLastName(event.target.value);
  }

  function handlePhoneNumberChange(event: ChangeEvent<HTMLInputElement>) {
    setPhoneNumber(event.target.value);
  }

  function handleRoleChange(
    event:
      | ChangeEvent<
          Omit<HTMLInputElement, "value"> & {
            value: string;
          }
        >
      | (Event & {
          target: {
            value: string;
            name: string;
          };
        }),
  ) {
    setRole(event.target.value);
  }

  function handleSubmit() {
    const data = {
      email: email,
      firstName: firstName,
      lastName: lastName,
      phoneNumber: phoneNumber,
      role: role,
    };

    setIsSubmitting(true);
    axios
      .post(`${import.meta.env.VITE_API_URL}/api/users`, data, {
        headers: {
          Authorization: `Bearer ${user?.accessToken}`,
        },
      })
      .then((_) => {
        addToast({
          message: "User added successfully",
          variant: "success",
        });
        navigate("/manage-users");
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
        setIsSubmitting(false);
      });
  }

  const isValidEmail = !email || isEmail(email);
  const isValidPhoneNumber = !phoneNumber || isMobilePhone(phoneNumber);

  return (
    <Box
      display="flex"
      flexDirection="column"
      width="400px"
      height="500px"
      color="white"
      boxShadow={4}
      justifyContent="center"
      alignItems="center"
      gap={2}
    >
      <TextField
        required
        label="Email Address"
        value={email}
        error={!isValidEmail}
        helperText={!isValidEmail ? "Invalid email address" : undefined}
        onChange={handleEmailChange}
      />
      <TextField
        label="First Name"
        value={firstName}
        onChange={handleFirstNameChange}
      />
      <TextField
        label="Last Name"
        value={lastName}
        onChange={handleLastNameChange}
      />
      <TextField
        label="Phone Number"
        error={!isValidPhoneNumber}
        helperText={!isValidPhoneNumber ? "Invalid phone number" : undefined}
        value={phoneNumber}
        onChange={handlePhoneNumberChange}
      />
      <FormControl sx={{ width: "150px" }}>
        <InputLabel>Role</InputLabel>
        <Select
          required
          label="Role"
          value={role}
          onChange={(event) => handleRoleChange(event)}
        >
          {Options.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {option.label}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
      <Button
        disabled={
          !(!!email && !!role && isValidEmail && isValidPhoneNumber) ||
          isSubmitting
        }
        variant="contained"
        color="primary"
        onClick={handleSubmit}
      >
        Add User
      </Button>
    </Box>
  );
}
