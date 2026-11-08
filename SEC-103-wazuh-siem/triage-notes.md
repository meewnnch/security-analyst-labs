# SEC-103 — Failed Windows Logon Triage

## Environment

- Wazuh server (AIO): Ubuntu Server 26.04.1 VM `nw-siem` at `192.168.169.140`
- Wazuh agent: Windows 11 VM `VM-Window11`, agent ID **001**, IP `192.168.169.11`, Wazuh **4.14.10**, status Active

## Test performed

On the Windows lab VM, an intentional failed logon was generated with `runas` against the **nonexistent** local account `fakeuser` (wrong password each attempt). This exercises local Secondary Logon failure handling only. It does not authenticate a real user and is not a remote attack simulation.

## Search used

In Wazuh **Explore → Discover**, index pattern `wazuh-alerts-*`, widened time range (Last 24 hours / covering the test window), DQL:

```text
agent.id: "001" AND data.win.system.eventID: "4625"
```

Result in the triage window: **7 hits** (see [`screenshots/03-discover-seven-4625-events.png`](screenshots/03-discover-seven-4625-events.png)).

## Evidence from one inspected event

Source: [`screenshots/04-fakeuser-event-details.png`](screenshots/04-fakeuser-event-details.png) and [`screenshots/05-failed-logon-4625-full.png`](screenshots/05-failed-logon-4625-full.png)

| Field | Value |
|-------|--------|
| Windows Event ID | 4625 |
| Agent | 001 / VM-Window11 / 192.168.169.11 |
| Manager | nw-siem |
| Target username | fakeuser |
| Initiating account | Administrator |
| Logon type | 2 (interactive) |
| Source address | ::1 (local loopback) |
| Wazuh rule | Logon Failure - Unknown user or bad password |
| Example rule id / level (from expanded alert) | 60122 / level 5 |

Visible supporting codes in the expanded alert included failure status/substatus values consistent with unknown user / bad password (e.g. `0xc000006d` / `0xc0000064` style fields). Exact codes are screenshot evidence, not re-typed as secrets.

## Conclusion

The inspected **4625** alert matches the intentional local test against a nonexistent account (`fakeuser`), initiated interactively from the same host (`::1`, logon type 2).

**Do not over-claim:**

- The **7** Discover matches are **not** equated here to seven independently confirmed test attempts.
- This event set is **not** evidence of remote compromise or an external attack.
- Other nearby alerts (for example Group Policy / connectivity events such as Windows **1129**) are separate issues and were not used as failed-logon proof.

## Outcome

Verified end-to-end: Windows Security log → Wazuh agent → searchable alert in Discover → field-level triage documenting intentional lab activity.
