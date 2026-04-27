const fs = require('fs');
const path = require('path');

const filesToUpdate = [
  {
    path: 'core/interceptors/auth.interceptor.ts',
    replacements: [
      { from: "alert('Access denied');", to: "toast.error('Access denied');" },
      { from: "alert('Server error occurred');", to: "toast.error('Server error occurred');" }
    ]
  },
  {
    path: 'features/dashboard/pages/question-bank/question-bank.component.ts',
    replacements: [
      { from: "alert('Question added to bank');", to: "toast.success('Question added to bank');" },
      { from: "alert('Failed to create question');", to: "toast.error('Failed to create question');" }
    ]
  },
  {
    path: 'features/interviews/pages/create-interview/create-interview.component.ts',
    replacements: [
      { from: "alert('Failed');", to: "toast.error('Failed');" }
    ]
  },
  {
    path: 'features/interviews/pages/schedule-interview/schedule-interview.component.ts',
    replacements: [
      { from: "alert('End time must be after start time');", to: "toast.warning('End time must be after start time');" },
      { from: "alert(res?.message || 'Interview Scheduled');", to: "toast.success(res?.message || 'Interview Scheduled');" },
      { from: "alert(err?.error?.message || 'Schedule failed');", to: "toast.error(err?.error?.message || 'Schedule failed');" }
    ]
  },
  {
    path: 'features/interviews/pages/assign-questions/assign-questions.component.ts',
    replacements: [
      { from: "alert('Please select at least one question');", to: "toast.warning('Please select at least one question');" },
      { from: "alert(res?.message || 'Questions Assigned');", to: "toast.success(res?.message || 'Questions Assigned');" },
      { from: "alert('Failed to assign questions');", to: "toast.error('Failed to assign questions');" }
    ]
  },
  {
    path: 'features/interviews/pages/invite-candidate/invite-candidate.component.ts',
    replacements: [
      { from: "alert('Invite Sent Successfully');", to: "toast.success('Invite Sent Successfully');" },
      { from: "alert('Failed to send invite');", to: "toast.error('Failed to send invite');" }
    ]
  },
  {
    path: 'features/dashboard/components/create-tenant/create-tenant.component.ts',
    replacements: [
      { from: "alert('Tenant created successfully');", to: "toast.success('Tenant created successfully');" },
      { from: "alert('Failed to create tenant');", to: "toast.error('Failed to create tenant');" }
    ]
  },
  {
    path: 'features/dashboard/components/create-hr/create-hr.component.ts',
    replacements: [
      { from: "alert('HR User created successfully');", to: "toast.success('HR User created successfully');" },
      { from: "alert(err?.error?.message || 'Failed to create HR user');", to: "toast.error(err?.error?.message || 'Failed to create HR user');" }
    ]
  },
  {
    path: 'features/dashboard/pages/hr-interviews/hr-interviews.component.ts',
    replacements: [
      { from: "alert('Interview stopped successfully');", to: "toast.success('Interview stopped successfully');" },
      { from: "alert('Error stopping interview');", to: "toast.error('Error stopping interview');" },
      { from: "alert('Interview removed successfully');", to: "toast.success('Interview removed successfully');" },
      { from: "alert('Error removing interview');", to: "toast.error('Error removing interview');" },
      { from: "alert('Invite link copied to clipboard!');", to: "toast.success('Invite link copied to clipboard!');" }
    ]
  },
  {
    path: 'features/dashboard/components/candidate-interviews/candidate-interviews.component.ts',
    replacements: [
      { from: "alert('Unable to start interview');", to: "toast.error('Unable to start interview');" }
    ]
  },
  {
    path: 'features/dashboard/pages/interview-monitor/interview-monitor.component.ts',
    replacements: [
      { from: "alert('🔔 The candidate has submitted their interview code!');", to: "toast.info('🔔 The candidate has submitted their interview code!');" },
      { from: "alert(\"Candidate is not online yet.\");", to: "toast.warning(\"Candidate is not online yet.\");" },
      { from: "alert('Interview stopped successfully');", to: "toast.success('Interview stopped successfully');" },
      { from: "alert('Error stopping interview');", to: "toast.error('Error stopping interview');" }
    ]
  },
  {
    path: 'features/interviews/pages/interview-room/interview-room.component.ts',
    replacements: [
      { from: "alert('Invalid interview token');", to: "toast.error('Invalid interview token');" },
      { from: "alert('Unable to start interview');", to: "toast.error('Unable to start interview');" },
      { from: "alert('Unable to resume interview');", to: "toast.error('Unable to resume interview');" },
      { from: "alert('Time is up! Auto submitting...');", to: "toast.warning('Time is up! Auto submitting...');" },
      { from: "alert('This interview has been stopped by the HR. Redirecting to your results...');", to: "toast.info('This interview has been stopped by the HR. Redirecting to your results...');" },
      { from: "alert('Submission successful!');", to: "toast.success('Submission successful!');" },
      { from: "alert('Submission failed');", to: "toast.error('Submission failed');" }
    ]
  },
  {
    path: 'features/auth/pages/login/login.component.ts',
    replacements: [
      { from: "alert('Login failed');", to: "toast.error('Login failed');" }
    ]
  },
  {
    path: 'features/auth/pages/register/register.component.ts',
    replacements: [
      { from: "alert(message);", to: "toast.success(message);" },
      { from: "alert(message || 'Registration failed');", to: "toast.error(message || 'Registration failed');" },
      { from: "alert(err.error);", to: "toast.success(err.error);" }
    ]
  }
];

const basePath = path.join('c:', 'Users', 'HP', 'OneDrive', 'Desktop', 'CodeInterviewPro', 'codeinterviewpro-ui', 'src', 'app');

filesToUpdate.forEach(fileDef => {
  const fullPath = path.join(basePath, fileDef.path);
  if (!fs.existsSync(fullPath)) {
    console.log('File not found:', fullPath);
    return;
  }

  let content = fs.readFileSync(fullPath, 'utf8');
  let changed = false;

  fileDef.replacements.forEach(rep => {
    if (content.includes(rep.from)) {
      content = content.split(rep.from).join(rep.to);
      changed = true;
    } else {
        console.log(`Could not find "${rep.from}" in ${fileDef.path}`);
    }
  });

  if (changed) {
    if (!content.includes("import { toast }")) {
      content = "import { toast } from 'src/app/core/services/toast';\n" + content;
    }
    fs.writeFileSync(fullPath, content, 'utf8');
    console.log('Updated:', fileDef.path);
  }
});
