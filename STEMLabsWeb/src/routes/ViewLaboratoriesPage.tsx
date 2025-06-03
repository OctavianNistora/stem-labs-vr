import {
  DataGrid,
  type GridColDef,
  type GridRowParams,
  useGridApiRef,
} from "@mui/x-data-grid";
import { Box, Button } from "@mui/material";
import { Link, useNavigate } from "react-router";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";

type laboratoryRowType = {
  id: number;
  name: string;
  sceneId: number;
  checkListStepCount: number;
};

const columns: GridColDef[] = [
  {
    field: "id",
    align: "center",
    headerName: "ID",
    headerAlign: "center",
    type: "number",
  },
  {
    field: "name",
    align: "center",
    headerName: "Laboratory Name",
    headerAlign: "center",
  },
  {
    field: "sceneId",
    align: "center",
    headerName: "Scene ID",
    headerAlign: "center",
  },
  {
    field: "checkListStepCount",
    align: "center",
    headerName: "No. of Checklist Steps",
    headerAlign: "center",
  },
];

export default function ViewLaboratoriesPage() {
  const [rows, setRows] = useState<laboratoryRowType[]>([]);
  const apiRef = useGridApiRef();
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);
  const navigate = useNavigate();

  useEffect(() => {
    axiosRequestWithAutoReauth(
      {
        method: "GET",
        url: `${import.meta.env.VITE_API_URL}/api/laboratories`,
        headers: {
          Authorization: `Bearer ${user?.accessToken}`,
        },
      },
      setUser,
    )
      .then((response) => {
        const rowsData = response.data.map((lab: any) => ({
          id: lab.id,
          name: lab.name,
          sceneId: lab.sceneId,
          checkListStepCount: lab.checkListStepCount,
        }));

        setRows(rowsData);
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      });
  }, []);

  useEffect(() => {
    apiRef?.current?.autosizeColumns({
      includeHeaders: true,
      includeOutliers: true,
      expand: true,
    });
  });

  function handleRowClick(params: GridRowParams) {
    navigate(`${params.id}`);
  }

  return (
    <Box
      display="flex"
      flexDirection="column"
      width="1400px"
      height="800px"
      boxShadow={4}
    >
      <Button component={Link} to="-1">
        Add New Laboratory
      </Button>
      {rows.length > 0 ? (
        <Box flexGrow={1}>
          <DataGrid
            apiRef={apiRef}
            rows={rows}
            columns={columns}
            disableRowSelectionOnClick
            disableColumnResize
            autoPageSize
            onRowClick={handleRowClick}
          />
        </Box>
      ) : (
        <></>
      )}
    </Box>
  );
}
