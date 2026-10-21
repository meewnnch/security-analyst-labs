# SEC-101: SSH Brute-Force Compromise on web01

**Organization:** Northwind Wellness (simulated)
**Analyst:** Nutnicha Rungruangpiroj
**Severity:** Critical (confirmed account compromise)
**Data:** Simulated `auth.log` created for training purposes

## Summary
On Oct 19, 2026, the web server `web01` was targeted by SSH brute-force attacks from three external IP addresses. One attacker, `45.95.147.12`, successfully logged in as the `admin` account with a password at 23:55:29.

## Findings
| Source IP | Failed logins | Successful login |
|---|---|---|
| 185.220.101.34 | 420 | No |
| 45.95.147.12 | 160 | **Yes (admin)** |
| 103.152.18.77 | 35 | No |

Normal activity: the `deploy` account logs in with SSH keys from internal addresses `10.0.0.5` and `10.0.0.6`.

## Timeline (45.95.147.12)
| Time | Event |
|---|---|
| Oct 19 23:01:01 | First failed login attempt |
| Oct 19 23:07:21 | Last failed login attempt |
| Oct 19 23:55:29 | Successful password login as `admin` |

About 54 minutes passed from the first attempt to the compromise. The attacker paused for 48 minutes and then logged in on the first try after returning, which suggests the password may have been obtained another way.

## Investigation Commands
```bash
# Count failed logins per IP
grep "Failed password" auth.log | awk '{print $(NF-3)}' | sort | uniq -c | sort -rn

# Find successful logins from outside the internal network
grep "Accepted" auth.log | grep -v "10.0.0."

# Build the attacker timeline
grep "45.95.147.12" auth.log | head -1
grep "45.95.147.12" auth.log | tail -2
```

## Containment (first hour)
1. Preserve logs as evidence before making changes.
2. Disable the `admin` account and end its active sessions.
3. Block all three attacker IPs at the firewall.
4. Isolate `web01` from the network to prevent lateral movement.

## Recommendations
1. **fail2ban:** Automatically ban an IP after a few failed logins, reducing hundreds of guesses to a handful. Best combined with other controls, since distributed attacks can rotate IPs.
2. **MFA for admin accounts:** Even with the correct password, the attacker could not have logged in without the second factor.
3. **Restrict SSH to the internal network or VPN:** Reduces the attack surface by removing SSH exposure to the internet while still allowing secure remote administration.

## Security+ (SY0-701) Concepts
- Password attacks and brute force (Chapter 4)
- Endpoint hardening (Chapter 11)
- Log analysis and incident response process (Chapter 14)
- Evidence preservation (Chapter 15)
