import {
  Box,
  Button,
  Divider,
  IconButton,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { Link, useNavigate, useParams } from "react-router";
import { useContext, useEffect, useState } from "react";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import type { IdDate } from "../types/IdDate.tsx";
import dateToFormattedString from "../helpers/dateToFormatedString.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";

type ChecklistStep = {
  statement: string;
  isCompleted: boolean;
};

type ReportDetails = {
  id: number;
  submitter: string;
  steps: ChecklistStep[];
  link: string;
};

export default function ViewRelatedReportPage() {
  const { sessionId, userId, reportId } = useParams();
  const [reportTabList, setReportTabList] = useState<IdDate[]>([]);
  const [reportDetailsList, setReportDetailsList] = useState<ReportDetails[]>(
    [],
  );
  const navigate = useNavigate();
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    if (!isStringPositiveInteger(userId)) {
      navigate("..");
      return;
    }

    fetchReportsList();
  }, []);

  useEffect(() => {
    if (!reportId) {
      return;
    }

    if (!isStringPositiveInteger(reportId)) {
      navigate(`../${userId}`);
    }

    if (
      reportDetailsList
        .map((report) => report.id.toString())
        .some((id) => id === reportId)
    ) {
      return;
    }

    fetchReportDetails();
  }, [reportId]);

  async function fetchReportsList() {
    try {
      const response = await axiosRequestWithAutoReauth(
        {
          method: "GET",
          url: `${import.meta.env.VITE_API_URL}/api/laboratory-sessions/${sessionId}/participants/${userId}/reports`,
          headers: {
            Authorization: `Bearer ${user?.accessToken}`,
          },
        },
        setUser,
      );

      setReportTabList(
        response.data
          .map((report: any) => ({
            id: report.id,
            date: new Date(report.submittedAt),
          }))
          .sort((a: IdDate, b: IdDate) => a.date.getTime() - b.date.getTime()),
      );
    } catch (error) {
      toastErrorMessageHandle(addToast, setUser, error);
    }
  }

  async function fetchReportDetails() {
    try {
      const response = await axiosRequestWithAutoReauth(
        {
          method: "GET",
          url: `${import.meta.env.VITE_API_URL}/api/laboratory-reports/${reportId}`,
          headers: {
            Authorization: `Bearer ${user?.accessToken}`,
          },
        },
        setUser,
      );

      const reportDetails: ReportDetails = {
        id: response.data.id,
        submitter: response.data.submitterFullName,
        steps: response.data.checklistSteps.map((step: any) => ({
          statement: step.statement,
          isCompleted: step.isCompleted,
        })),
        link: response.data.observationsImageLink,
      };

      setReportDetailsList((prev) => [...prev, reportDetails]);
    } catch (error) {
      toastErrorMessageHandle(addToast, setUser, error);
      navigate(`../${userId}`);
    }
  }

  const currentReportDetails = reportDetailsList.find(
    (report) => report.id.toString() === reportId,
  );

  return (
    <Box
      display="flex"
      flexDirection="column"
      width="800px"
      height="800px"
      bgcolor="white"
      boxShadow={4}
      overflow="visible"
      alignItems="center"
    >
      <Box width="100%" display="flex">
        <Box width={0} height={40} display="flex" flexDirection="row-reverse">
          <IconButton
            component={Link}
            to={user?.role.toLowerCase() === "student" ? "../.." : `..`}
          >
            <ArrowBackIcon />
          </IconButton>
        </Box>
        <Box width="100%" display="flex" flexDirection="column">
          <Typography variant="h4" marginY={2} textAlign="center">
            Select a report to view
          </Typography>
          <Divider sx={{ width: "100%" }} />
          <Box width="100%" height="100%">
            <Stack width="100%" height="100%" direction="row" overflow="auto">
              {reportTabList.map((report, index) => (
                <Box display="flex" key={report.id} whiteSpace="nowrap">
                  <Button
                    component={Link}
                    to={
                      (reportId == undefined ? "" : `../${userId}/`) +
                      `${report.id}`
                    }
                    replace
                  >
                    <Typography
                      color="text.primary"
                      fontWeight={
                        report.id.toString() == reportId ? "bold" : "normal"
                      }
                    >
                      {dateToFormattedString(report.date)}
                    </Typography>
                  </Button>
                  {index < reportTabList.length - 1 ? (
                    <Divider orientation="vertical" />
                  ) : (
                    <></>
                  )}
                </Box>
              ))}
            </Stack>
          </Box>
        </Box>
      </Box>
      <Divider sx={{ width: "100%" }} />
      {currentReportDetails ? (
        <Box
          width="100%"
          height="100%"
          display="flex"
          flexDirection="column"
          padding={2}
          overflow="hidden"
        >
          <TextField
            variant="standard"
            label="Submitter"
            value={currentReportDetails.submitter}
            slotProps={{
              input: {
                readOnly: true,
                disableUnderline: true,
              },
              inputLabel: {
                shrink: true,
              },
            }}
          />
          <Box display="flex" flexDirection="column" overflow="hidden">
            <Stack direction="column" overflow="auto">
              {currentReportDetails.steps.map((step, index) => (
                <TextField
                  variant="standard"
                  label={`Step ${index + 1}`}
                  value={step.statement}
                  multiline
                  color={step.isCompleted ? "success" : "error"}
                  focused
                  slotProps={{
                    input: {
                      readOnly: true,
                      disableUnderline: true,
                    },
                    inputLabel: {
                      shrink: true,
                    },
                  }}
                />
              ))}
            </Stack>
            <Typography
              color={currentReportDetails.link ? "success" : "error"}
              marginY={1}
            >
              {currentReportDetails.link
                ? "Observations image:"
                : "No observations image"}
            </Typography>
            {currentReportDetails.link ? (
              <Box display="flex" height="200px">
                <Box height="100%" marginRight="auto" boxShadow={2}>
                  <Link to={currentReportDetails.link} target="_blank">
                    <img
                      src={currentReportDetails.link}
                      alt="Observations"
                      style={{
                        maxHeight: "100%",
                        objectFit: "cover",
                      }}
                    />
                  </Link>
                </Box>
              </Box>
            ) : (
              <></>
            )}
          </Box>
        </Box>
      ) : (
        <></>
      )}
    </Box>
  );
}
