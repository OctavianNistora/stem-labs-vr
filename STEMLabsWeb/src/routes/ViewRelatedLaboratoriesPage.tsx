import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Typography,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import axios from "axios";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import type { IdName } from "../types/IdName.tsx";
import Divider from "@mui/material/Divider";
import { Link } from "react-router";

export default function ViewRelatedLaboratoriesPage() {
  const [laboratories, setLaboratories] = useState<IdName[]>([]);
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    fetchLaboratories();
  }, []);

  async function fetchLaboratories() {
    if (!user) {
      return;
    }

    try {
      let response;
      if (user.role.toLowerCase() === "admin") {
        response = await axios.get<IdName[]>(
          `${import.meta.env.VITE_API_URL}/api/laboratories/simplified`,
          {
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          },
        );
      } else {
        response = await axios.get<IdName[]>(
          `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/related-laboratories`,
          {
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          },
        );
      }
      setLaboratories(response.data);
    } catch (error) {
      toastErrorMessageHandle(addToast, setUser, error);
    }
  }

  return (
    <Box
      display="flex"
      flexDirection="column"
      width="600px"
      height="800px"
      bgcolor="white"
      boxShadow={4}
      overflow="auto"
      alignItems="center"
    >
      <Typography variant="h4" marginY={2}>
        Select a laboratory
      </Typography>
      <List sx={{ width: "100%" }}>
        {laboratories.map((laboratory) => (
          <>
            <Divider />
            <ListItem key={laboratory.id} disablePadding>
              <ListItemButton component={Link} to={laboratory.id.toString()}>
                <ListItemText primary={laboratory.name} />
              </ListItemButton>
            </ListItem>
          </>
        ))}
      </List>
    </Box>
  );
}
