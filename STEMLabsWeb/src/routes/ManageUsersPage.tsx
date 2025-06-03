import { Box, Button, IconButton, Stack } from "@mui/material";
import { DataGrid, type GridColDef, useGridApiRef } from "@mui/x-data-grid";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import RemoveCircleIcon from "@mui/icons-material/RemoveCircle";
import UpgradeIcon from "@mui/icons-material/Upgrade";
import { Link } from "react-router";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";

type userRowType = {
  uid: number;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  email: string;
  createdAt: string;
  role: string;
  isUpdating: boolean;
};

export default function ManageUsersPage() {
  const [rows, setRows] = useState<userRowType[]>([]);
  const apiRef = useGridApiRef();
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    if (!user) {
      return;
    }

    axiosRequestWithAutoReauth(
      {
        method: "GET",
        url: `${import.meta.env.VITE_API_URL}/api/users`,
        headers: {
          Authorization: `Bearer ${user.accessToken}`,
        },
      },
      setUser,
    )
      .then((response) => {
        const rowsData = response.data.map((user: any) => ({
          uid: user.uid,
          firstName: user.firstName,
          lastName: user.lastName,
          phoneNumber: user.phoneNumber,
          email: user.email,
          createdAt: user.createdAt,
          role: user.role,
          isUpdating: false,
        })) as userRowType[];

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

  function handleUpdateUserRole(uid: number, role: string) {
    setRows((prevRows) =>
      prevRows.map((row) =>
        row.uid === uid ? { ...row, isUpdating: true } : row,
      ),
    );
    axiosRequestWithAutoReauth(
      {
        method: "PUT",
        url: `${import.meta.env.VITE_API_URL}/api/users/${uid}/role`,
        data: role,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user?.accessToken}`,
        },
      },
      setUser,
    )
      .then((_) => {
        setRows((prevRows) =>
          prevRows.map((row) =>
            row.uid === uid ? { ...row, role: role } : row,
          ),
        );
        addToast({
          message: `User with ID ${uid} now has role ${role}.`,
          variant: "success",
        });
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      })
      .finally(() => {
        setRows((prevRows) =>
          prevRows.map((row) =>
            row.uid === uid ? { ...row, isUpdating: false } : row,
          ),
        );
      });
  }

  function handleDeleteUser(uid: number) {
    setRows((prevRows) =>
      prevRows.map((row) =>
        row.uid === uid ? { ...row, isUpdating: true } : row,
      ),
    );
    axiosRequestWithAutoReauth(
      {
        method: "DELETE",
        url: `${import.meta.env.VITE_API_URL}/api/users/${uid}`,
        headers: {
          Authorization: `Bearer ${user?.accessToken}`,
        },
      },
      setUser,
    )
      .then((_) => {
        setRows((prevRows) => prevRows.filter((row) => row.uid !== uid));
        addToast({
          message: `User with ID ${uid} has been deleted.`,
          variant: "success",
        });
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
        setRows((prevRows) =>
          prevRows.map((row) =>
            row.uid === uid ? { ...row, isUpdating: false } : row,
          ),
        );
      });
  }

  const columns: GridColDef<userRowType>[] = [
    {
      field: "uid",
      align: "center",
      headerName: "ID",
      headerAlign: "center",
      type: "number",
    },
    {
      field: "firstName",
      align: "center",
      headerName: "First Name",
      headerAlign: "center",
    },
    {
      field: "lastName",
      align: "center",
      headerName: "Last Name",
      headerAlign: "center",
    },
    {
      field: "phoneNumber",
      align: "center",
      headerName: "Phone Number",
      headerAlign: "center",
    },
    {
      field: "email",
      align: "center",
      headerName: "Email",
      headerAlign: "center",
    },
    {
      field: "createdAt",
      align: "center",
      headerName: "Created At",
      headerAlign: "center",
      type: "dateTime",
      valueGetter: (value) => new Date(value),
    },
    {
      field: "role",
      align: "center",
      headerName: "Role",
      headerAlign: "center",
    },
    {
      field: "actions",
      type: "actions",
      renderCell: (params) => {
        return (
          <Stack direction="row" justifyContent="right">
            {params.row.role.toLowerCase() == "professor" ? (
              <IconButton
                title="Demote to Student"
                onClick={() => handleUpdateUserRole(params.row.uid, "Student")}
              >
                <UpgradeIcon
                  color="primary"
                  sx={{ transform: "rotate(180deg)" }}
                />
              </IconButton>
            ) : (
              <Box width="40px" />
            )}
            {params.row.role.toLowerCase() == "student" ? (
              <IconButton
                title="Promote to Professor"
                onClick={() =>
                  handleUpdateUserRole(params.row.uid, "Professor")
                }
              >
                <UpgradeIcon color="primary" />
              </IconButton>
            ) : (
              <Box width="40px" />
            )}
            <IconButton
              title="Delete User"
              onClick={() => handleDeleteUser(params.row.uid)}
            >
              <RemoveCircleIcon color="error" />
            </IconButton>
          </Stack>
        );
      },
    },
  ];

  return (
    <Box
      display="flex"
      flexDirection="column"
      width="1400px"
      height="800px"
      boxShadow={4}
    >
      <Button component={Link} to="/add-user">
        Add New User
      </Button>
      {rows.length > 0 ? (
        <Box flexGrow={1}>
          <DataGrid
            apiRef={apiRef}
            rows={rows}
            getRowId={(row) => row.uid}
            columns={columns}
            disableRowSelectionOnClick
            disableColumnResize
            autoPageSize
          />
        </Box>
      ) : (
        <></>
      )}
    </Box>
  );
}
