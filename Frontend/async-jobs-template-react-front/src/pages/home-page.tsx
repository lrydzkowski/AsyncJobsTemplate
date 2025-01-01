import { useMsal } from '@azure/msal-react';
import { jobsClient } from '../api/jobs-client';
import { loginRequest } from '../auth/msal-config';

export function HomePage() {
  const { instance, accounts } = useMsal();

  console.log(accounts);

  const handleSignOut = async () => {
    await instance.logoutRedirect();
  };

  const sendRequest = async () => {
    const acquiringTokenResult = await instance.acquireTokenSilent({ ...loginRequest, account: accounts[0] });
    const response = await jobsClient.getJob(acquiringTokenResult.accessToken, 'd92ff174-3071-4209-9fc7-e68941bdd13c');
    console.log(response);
  };

  return (
    <>
      <p>Home Page</p>
      <button onClick={handleSignOut}>Sign Out</button>
      <button onClick={sendRequest}>Send Request</button>
    </>
  );
}
