import { useNavigate, useParams } from "react-router";
import {
  Box,
  Button,
  CircularProgress,
  IconButton,
  Stack,
  TextField,
} from "@mui/material";
import { type ChangeEvent, useContext, useEffect, useState } from "react";
import axios from "axios";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import RemoveIcon from "@mui/icons-material/Remove";
import AddIcon from "@mui/icons-material/Add";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";

export default function ManageLaboratoryPage() {
  const [name, setName] = useState("");
  const [sceneId, setSceneId] = useState("");
  const [checkListStepList, setCheckListStepList] = useState<string[]>([""]);
  const [isReceivingData, setIsReceivingData] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { laboratoryId } = useParams();
  const navigate = useNavigate();
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    if (!isStringPositiveInteger(laboratoryId) && laboratoryId != "-1") {
      navigate("..");
      return;
    }

    if (laboratoryId == "-1") {
      setIsReceivingData(false);
      return;
    }

    axios
      .get(`${import.meta.env.VITE_API_URL}/api/laboratories/${laboratoryId}`, {
        headers: {
          Authorization: `Bearer ${user?.accessToken}`,
        },
      })
      .then((response) => {
        const lab = response.data;
        setName(lab.name);
        setSceneId(lab.sceneId.toString());
        setCheckListStepList(lab.steps);
        setIsReceivingData(false);
      })
      .catch((error) => {
        navigate("..");
        toastErrorMessageHandle(addToast, setUser, error);
      });
  }, []);

  function handleNameChange(event: ChangeEvent<HTMLInputElement>) {
    setName(event.target.value);
  }

  function handleSceneIdChange(event: ChangeEvent<HTMLInputElement>) {
    setSceneId(event.target.value);
  }

  function handleCheckListStepList(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
    index: number,
  ) {
    setCheckListStepList((prevSteps) => {
      const newSteps = [...prevSteps];
      newSteps[index] = event.target.value;
      return newSteps;
    });
  }

  function handleSubmit() {
    const data = {
      name: name,
      sceneId: parseInt(sceneId),
      steps: checkListStepList,
    };

    setIsSubmitting(true);
    if (laboratoryId == "-1") {
      axios
        .post(`${import.meta.env.VITE_API_URL}/api/laboratories`, data, {
          headers: {
            Authorization: `Bearer ${user?.accessToken}`,
          },
        })
        .then(() => {
          navigate("..");
        })
        .catch((error) => {
          toastErrorMessageHandle(addToast, setUser, error);
        })
        .finally(() => {
          setIsSubmitting(false);
        });
    } else {
      axios
        .put(
          `${import.meta.env.VITE_API_URL}/api/laboratories/${laboratoryId}`,
          data,
          {
            headers: {
              Authorization: `Bearer ${user?.accessToken}`,
            },
          },
        )
        .then(() => {
          navigate("..");
        })
        .catch((error) => {
          toastErrorMessageHandle(addToast, setUser, error);
        })
        .finally(() => {
          setIsSubmitting(false);
        });
    }
  }

  function addCheckListStep() {
    setCheckListStepList((prevSteps) => [...prevSteps, ""]);
  }

  function removeLastCheckListStep() {
    setCheckListStepList((prevSteps) =>
      prevSteps.slice(0, prevSteps.length - 1),
    );
  }

  return (
    <Box
      display="flex"
      flexDirection="column"
      width="500px"
      height="800px"
      color="white"
      boxShadow={4}
      justifyContent="center"
      alignItems="center"
      gap={2}
      paddingX={3}
    >
      <Button
        disabled={isReceivingData || isSubmitting || !name || !sceneId}
        fullWidth
        variant="contained"
        onClick={handleSubmit}
      >
        {isSubmitting ? (
          <CircularProgress />
        ) : laboratoryId == "-1" ? (
          "Add New Laboratory"
        ) : (
          "Update Laboratory"
        )}
      </Button>
      <TextField
        fullWidth
        label="Laboratory Name"
        value={name}
        onChange={handleNameChange}
      />
      <TextField
        fullWidth
        type="number"
        slotProps={{ htmlInput: { min: 0 } }}
        label="Scene ID"
        value={sceneId}
        onChange={handleSceneIdChange}
      />
      <Box width="100%" height={560} overflow="auto">
        {checkListStepList.map((step, index) => (
          <TextField
            fullWidth
            key={index}
            label={`Checklist Step ${index + 1}`}
            value={step}
            onChange={(event) => handleCheckListStepList(event, index)}
            sx={{ marginY: 1 }}
          />
        ))}
        <Stack direction="row-reverse">
          <IconButton>
            <AddIcon onClick={addCheckListStep} />
          </IconButton>
          {checkListStepList.length > 1 ? (
            <IconButton>
              <RemoveIcon onClick={removeLastCheckListStep} />
            </IconButton>
          ) : (
            <></>
          )}
        </Stack>
      </Box>
    </Box>
  );
}
